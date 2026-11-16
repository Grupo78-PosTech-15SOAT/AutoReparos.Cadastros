# Fila de notificações do Cadastros (a fila pertence ao consumidor): o OS publica saga.lembrete.v1
# (Sqs:CadastrosQueueUrl do OS) e o Cadastros consome em Notificacoes:FilaUrl, com DLQ em Notificacoes:DlqUrl.
resource "aws_sqs_queue" "notificacoes_dlq" {
  name                      = "autoreparos-cadastros-notificacoes-dlq"
  message_retention_seconds = 1209600
  sqs_managed_sse_enabled   = true
}

resource "aws_sqs_queue" "notificacoes" {
  name                       = "autoreparos-cadastros-notificacoes"
  visibility_timeout_seconds = 90
  message_retention_seconds  = 345600
  receive_wait_time_seconds  = 20
  sqs_managed_sse_enabled    = true
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.notificacoes_dlq.arn
    maxReceiveCount     = 5
  })
}
