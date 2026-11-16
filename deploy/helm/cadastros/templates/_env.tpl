{{/* Configuração específica do Cadastros: ConfigMap do chart e Secret cadastros-segredos criado pelo job deploy. */}}
{{- define "servico.env" -}}
- name: OpenTelemetry__ServiceName
  value: {{ .Values.telemetria.nomeServico | quote }}
- name: OpenTelemetry__Endpoint
  value: {{ .Values.telemetria.otlpEndpoint | quote }}
{{- range list "Jwt__ExpiryHours" "Jwt__Issuer" "Notificacoes__FilaUrl" "Notificacoes__DlqUrl" "SeedUser__Email" "SendGrid__FromEmail" "SendGrid__FromName" }}
- name: {{ . }}
  valueFrom:
    configMapKeyRef:
      name: {{ include "servico.nome" $ }}-config
      key: {{ . }}
{{- end }}
{{- range list "Jwt__ChavePrivadaPem" "Interno__Chave" "SeedUser__Password" "SendGrid__ApiKey" }}
- name: {{ . }}
  valueFrom:
    secretKeyRef:
      name: {{ include "servico.nome" $ }}-segredos
      key: {{ . }}
{{- end }}
{{- end }}
