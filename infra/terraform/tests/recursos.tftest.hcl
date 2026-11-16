# C11: ECR, filas de entrada com DLQ, role IRSA presa ao ServiceAccount cadastros/cadastros e segredo da database do Cadastros.
# Roda sem AWS: provider aws simulado e parâmetros SSM da plataforma substituídos por valores de exemplo.

mock_provider "aws" {
  mock_data "aws_caller_identity" {
    defaults = {
      account_id = "198903849859"
    }
  }
}

override_data {
  target = data.aws_ssm_parameter.oidc_provider_arn
  values = {
    insecure_value = "arn:aws:iam::198903849859:oidc-provider/oidc.eks.us-east-1.amazonaws.com/id/EXEMPLO"
  }
}

override_data {
  target = data.aws_ssm_parameter.oidc_provider_url
  values = {
    insecure_value = "oidc.eks.us-east-1.amazonaws.com/id/EXEMPLO"
  }
}

override_data {
  target = data.aws_ssm_parameter.database_address
  values = {
    insecure_value = "autoreparos-db.exemplo.us-east-1.rds.amazonaws.com"
  }
}


override_data {
  target = data.aws_ssm_parameter.database_name
  values = {
    insecure_value = "autoreparos_db"
  }
}

override_data {
  target = data.aws_ssm_parameter.subnets_privadas
  values = {
    insecure_value = "subnet-a,subnet-b"
  }
}

override_data {
  target = data.aws_subnet.privadas
  values = {
    cidr_block = "10.0.1.0/24"
  }
}
run "recursos_do_cadastros" {
  command = apply

  assert {
    condition     = aws_sqs_queue.notificacoes.name == "autoreparos-cadastros-notificacoes" && aws_sqs_queue.notificacoes_dlq.name == "autoreparos-cadastros-notificacoes-dlq"
    error_message = "A fila de entrada autoreparos-cadastros-notificacoes precisa da DLQ autoreparos-cadastros-notificacoes-dlq."
  }

  assert {
    condition     = jsondecode(aws_sqs_queue.notificacoes.redrive_policy).deadLetterTargetArn == aws_sqs_queue.notificacoes_dlq.arn && jsondecode(aws_sqs_queue.notificacoes.redrive_policy).maxReceiveCount > 0
    error_message = "A redrive_policy de autoreparos-cadastros-notificacoes precisa apontar para autoreparos-cadastros-notificacoes-dlq com maxReceiveCount."
  }

  assert {
    condition     = aws_ecr_repository.servico.name == "autoreparos-cadastros"
    error_message = "O ECR precisa se chamar autoreparos-cadastros."
  }

  assert {
    condition     = aws_iam_role.irsa.name == "autoreparos-cadastros-irsa"
    error_message = "A role IRSA precisa se chamar autoreparos-cadastros-irsa."
  }

  assert {
    condition     = aws_iam_role.irsa.permissions_boundary == "arn:aws:iam::198903849859:policy/autoreparos-servico-boundary"
    error_message = "A role IRSA precisa do permissions boundary autoreparos-servico-boundary."
  }

  assert {
    condition     = jsondecode(aws_iam_role.irsa.assume_role_policy).Statement[0].Principal.Federated == "arn:aws:iam::198903849859:oidc-provider/oidc.eks.us-east-1.amazonaws.com/id/EXEMPLO"
    error_message = "A trust da role IRSA precisa confiar no provedor OIDC do cluster lido do SSM."
  }

  assert {
    condition     = jsondecode(aws_iam_role.irsa.assume_role_policy).Statement[0].Condition.StringEquals["oidc.eks.us-east-1.amazonaws.com/id/EXEMPLO:sub"] == "system:serviceaccount:cadastros:cadastros"
    error_message = "A trust da role IRSA precisa ficar presa ao ServiceAccount cadastros do namespace cadastros."
  }

  assert {
    condition     = length(jsondecode(aws_iam_role.irsa.assume_role_policy).Statement) == 1
    error_message = "A trust da role IRSA não pode ter outras declarações."
  }

  assert {
    condition     = aws_secretsmanager_secret.database.name == "autoreparos/production/cadastros/database"
    error_message = "O segredo da database precisa se chamar autoreparos/production/cadastros/database."
  }

  assert {
    condition = (
      jsondecode(aws_secretsmanager_secret_version.database.secret_string).username == "cadastros_app" &&
      jsondecode(aws_secretsmanager_secret_version.database.secret_string).dbname == "autoreparos_db" &&
      jsondecode(aws_secretsmanager_secret_version.database.secret_string).host == "autoreparos-db.exemplo.us-east-1.rds.amazonaws.com" &&
      jsondecode(aws_secretsmanager_secret_version.database.secret_string).port == 5432
    )
    error_message = "O segredo precisa de username cadastros_app, dbname autoreparos_db, host do SSM e porta 5432."
  }

  assert {
    condition = strcontains(
      jsondecode(aws_secretsmanager_secret_version.database.secret_string).connectionString,
      "Host=autoreparos-db.exemplo.us-east-1.rds.amazonaws.com;Port=5432;Database=autoreparos_db;Username=cadastros_app;Password=${random_password.database.result};SSL Mode=Prefer;"
    )
    error_message = "A connectionString do segredo precisa usar o host, a database, o usuário e a senha do próprio segredo."
  }

  assert {
    condition     = output.cidrs_privados == "10.0.1.0/24"
    error_message = "Os CIDRs das sub-redes privadas precisam sair do SSM para a NetworkPolicy."
  }
}
