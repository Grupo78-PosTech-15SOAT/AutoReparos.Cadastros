output "ecr_url" {
  description = "URL do repositório ECR do serviço (a tag da imagem é o SHA do commit)."
  value       = aws_ecr_repository.servico.repository_url
}

output "irsa_role_arn" {
  description = "ARN da role IRSA anotada no ServiceAccount do chart."
  value       = aws_iam_role.irsa.arn
}

output "segredo_database_arn" {
  description = "ARN do segredo com a connection string do serviço."
  value       = aws_secretsmanager_secret.database.arn
}

output "fila_notificacoes_url" {
  description = "URL da fila de notificações."
  value       = aws_sqs_queue.notificacoes.url
}

output "fila_notificacoes_dlq_url" {
  description = "URL da DLQ de notificações."
  value       = aws_sqs_queue.notificacoes_dlq.url
}

output "cidrs_privados" {
  description = "CIDRs das sub-redes privadas, separados por vírgula, para a NetworkPolicy do chart."
  value       = join(",", sort(distinct([for subnet in data.aws_subnet.privadas : subnet.cidr_block])))
}
