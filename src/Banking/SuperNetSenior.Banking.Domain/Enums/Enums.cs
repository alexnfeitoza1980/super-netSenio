namespace SuperNetSenior.Banking.Domain.Enums;

public enum SituacaoCadastral { EmAnalise = 1, Ativo = 2, Bloqueado = 3, Encerrado = 4 }

public enum StatusConta { Criada = 1, Ativa = 2, Bloqueada = 3, Encerrada = 4 }

public enum StatusOperacao { Agendado = 1, EmProcessamento = 2, Liquidado = 3, Falha = 4, Cancelado = 5 }

public enum TipoLancamento { Credito = 1, Debito = 2, Reserva = 3, Estorno = 4 }

public enum TipoPosicao { RendaFixa = 1, RendaVariavel = 2, Fundos = 3, CDB = 4, TesouroDireto = 5 }

public enum StatusApolice { Proposta = 1, EmAnalise = 2, Emitida = 3, Cancelada = 4, Sinistrada = 5 }
