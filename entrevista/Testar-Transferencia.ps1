param(
    [Parameter(Mandatory)][uri]$Api,
    [Parameter(Mandatory)][securestring]$Token
)

$ErrorActionPreference = 'Stop'
if (-not $Api.IsLoopback -or $Api.Scheme -notin @('http', 'https')) {
    throw 'Use a API local de laboratório com banco descartável.'
}

Add-Type -AssemblyName System.Net.Http
$client = [System.Net.Http.HttpClient]::new()
$client.BaseAddress = [uri]($Api.AbsoluteUri.TrimEnd('/') + '/')
$client.Timeout = [TimeSpan]::FromSeconds(20)
$client.DefaultRequestHeaders.Authorization = [System.Net.Http.Headers.AuthenticationHeaderValue]::new(
    'Bearer', [System.Net.NetworkCredential]::new('', $Token).Password
)

function Novo-Corpo($Dados) {
    [System.Net.Http.StringContent]::new(
        (ConvertTo-Json -InputObject $Dados -Compress), [Text.Encoding]::UTF8, 'application/json'
    )
}

function Ler-Resposta($Resposta, [int[]]$Aceitos) {
    try {
        $status = [int]$Resposta.StatusCode
        if ($status -notin $Aceitos) {
            throw "HTTP $status. Esperado: $($Aceitos -join ', ')."
        }
        if ($Resposta.IsSuccessStatusCode) {
            $Resposta.Content.ReadAsStringAsync().GetAwaiter().GetResult() | ConvertFrom-Json
        }
    } finally {
        $Resposta.Dispose()
    }
}

function Enviar-Post($Rota, $Dados, [int[]]$Aceitos) {
    $corpo = Novo-Corpo $Dados
    try {
        Ler-Resposta ($client.PostAsync($Rota, $corpo).GetAwaiter().GetResult()) $Aceitos
    } finally {
        $corpo.Dispose()
    }
}

try {
    $origem = Enviar-Post 'api/contas' @{ numero = [guid]::NewGuid().ToString('N').Substring(0, 20); agencia = '0001' } 201
    $destino = Enviar-Post 'api/contas' @{ numero = [guid]::NewGuid().ToString('N').Substring(0, 20); agencia = '0001' } 201
    Enviar-Post "api/contas/$($origem.id)/movimentos" @{
        tipo = 1; valor = 100; chaveIdempotencia = [guid]::NewGuid().ToString('N')
    } 200 | Out-Null
    $dados = @{
        contaOrigemId = $origem.id; contaDestinoId = $destino.id
        valor = 30; chaveIdempotencia = [guid]::NewGuid().ToString('N')
    }

    $primeiroCorpo = Novo-Corpo $dados
    $segundoCorpo = Novo-Corpo $dados
    try {
        $primeiro = $client.PostAsync('api/transferencias', $primeiroCorpo)
        $segundo = $client.PostAsync('api/transferencias', $segundoCorpo)
        $respostas = @($primeiro.GetAwaiter().GetResult(), $segundo.GetAwaiter().GetResult())
        $vencedores = @($respostas | Where-Object IsSuccessStatusCode)
        if ($vencedores.Count -eq 0) { throw 'Nenhuma transferência concorrente foi aceita.' }
        $resultados = @($respostas | ForEach-Object { Ler-Resposta $_ @(200, 409) })
    } finally {
        $primeiroCorpo.Dispose()
        $segundoCorpo.Dispose()
    }

    $reenvio = Enviar-Post 'api/transferencias' $dados 200
    if (-not $reenvio.id -or @($resultados | Where-Object { $_.id -ne $reenvio.id }).Count -gt 0) {
        throw 'O reenvio deve recuperar a única transferência aceita.'
    }
    $dados.valor = 31
    Enviar-Post 'api/transferencias' $dados 409 | Out-Null

    $saldoOrigem = Ler-Resposta ($client.GetAsync("api/contas/$($origem.id)/saldo").GetAwaiter().GetResult()) 200
    $saldoDestino = Ler-Resposta ($client.GetAsync("api/contas/$($destino.id)/saldo").GetAwaiter().GetResult()) 200
    if ($saldoOrigem.saldo -ne 70 -or $saldoDestino.saldo -ne 30) {
        throw 'Os saldos devem refletir um único débito de 30 e um único crédito de 30.'
    }
    Write-Output "Transferência $($reenvio.id) confirmada. Saldos finais: 70 / 30."
} finally {
    $client.Dispose()
}
