{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "cati-supervisor-legacy-client.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "cati-supervisor-legacy-client.fullname" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- if contains $name .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}
{{- end -}}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "cati-supervisor-legacy-client.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Common labels
*/}}
{{- define "cati-supervisor-legacy-client.labels" -}}
helm.sh/chart: {{ include "cati-supervisor-legacy-client.chart" . }}
app: {{ include "cati-supervisor-legacy-client.name" . }}
{{ include "cati-supervisor-legacy-client.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ default .Chart.AppVersion .Values.appImage.tagOverride }}
version: {{ default .Chart.AppVersion .Values.appImage.tagOverride }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
app.confirmit.com/template: iis-app
app.confirmit.com/template-version: 12.7.0
{{- end -}}

{{/*
Selector labels
*/}}
{{- define "cati-supervisor-legacy-client.selectorLabels" -}}
app.kubernetes.io/name: {{ include "cati-supervisor-legacy-client.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/*
Selector labels for service. The reason why we have special selector for service that use old app label is to be compatible with Istio/Kiali
*/}}
{{- define "cati-supervisor-legacy-client.serviceSelectorLabels" -}}
app: {{ include "cati-supervisor-legacy-client.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/*
Create the name of the service account to use
*/}}
{{- define "cati-supervisor-legacy-client.serviceAccountName" -}}
{{- if .Values.serviceAccount.create -}}
    {{ default (include "cati-supervisor-legacy-client.fullname" .) .Values.serviceAccount.name }}
{{- else -}}
    {{ default "default" .Values.serviceAccount.name }}
{{- end -}}
{{- end -}}
