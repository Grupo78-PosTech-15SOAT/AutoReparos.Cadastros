# A plataforma (repositório pai) publica no SSM o que os serviços precisam; o serviço não lê o state dela (D-53).
locals {
  servico    = "cadastros"
  parametros = "/autoreparos/${var.ambiente}"
  conta      = data.aws_caller_identity.atual.account_id
}

data "aws_caller_identity" "atual" {}

data "aws_ssm_parameter" "oidc_provider_arn" {
  name = "${local.parametros}/eks/oidc_provider_arn"
}

data "aws_ssm_parameter" "oidc_provider_url" {
  name = "${local.parametros}/eks/oidc_provider_url"
}

data "aws_ssm_parameter" "database_address" {
  name = "${local.parametros}/database/address"
}

data "aws_ssm_parameter" "database_name" {
  name = "${local.parametros}/database/name"
}

# Sub-redes privadas da VPC: os CIDRs entram na NetworkPolicy do Cadastros (ALB interno e AuthLambda chegam por elas).
data "aws_ssm_parameter" "subnets_privadas" {
  name = "${local.parametros}/subnets/private"
}

data "aws_subnet" "privadas" {
  for_each = toset(split(",", data.aws_ssm_parameter.subnets_privadas.insecure_value))
  id       = each.value
}
