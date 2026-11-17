# Assinatura do tópico dos eventos da Execução (D-67, porta 2 de infra-ci-observabilidade): o consumidor assina a
# própria fila com raw delivery (o corpo chega igual ao envelope publicado) e filtro pelo atributo `tipo`:
# só a conclusão, que vira o e-mail "serviço concluído" (EventoNotificacao). O tópico é criado pelo Terraform da Execução, que precisa ser aplicado antes.
locals {
  topico_execucao_eventos = "arn:aws:sns:${var.regiao}:${local.conta}:autoreparos-execucao-eventos"
  tipos_assinados_execucao = [
    "execucao.concluida.v1",
  ]
}

resource "aws_sqs_queue_policy" "notificacoes_recebe_do_topico_execucao" {
  queue_url = aws_sqs_queue.notificacoes.id
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Sid       = "TopicoDaExecucaoEntregaNaFila"
      Effect    = "Allow"
      Principal = { Service = "sns.amazonaws.com" }
      Action    = "sqs:SendMessage"
      Resource  = aws_sqs_queue.notificacoes.arn
      Condition = { ArnEquals = { "aws:SourceArn" = local.topico_execucao_eventos } }
    }]
  })
}

resource "aws_sns_topic_subscription" "notificacoes_eventos_execucao" {
  topic_arn            = local.topico_execucao_eventos
  protocol             = "sqs"
  endpoint             = aws_sqs_queue.notificacoes.arn
  raw_message_delivery = true
  filter_policy_scope  = "MessageAttributes"
  filter_policy        = jsonencode({ tipo = local.tipos_assinados_execucao })

  depends_on = [aws_sqs_queue_policy.notificacoes_recebe_do_topico_execucao]
}
