using Jornada.Resiliencia.Lab10;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var roteiro = new Roteiro(Console.In, Console.Out, DateOnly.FromDateTime(DateTime.Today));
roteiro.Executar();
