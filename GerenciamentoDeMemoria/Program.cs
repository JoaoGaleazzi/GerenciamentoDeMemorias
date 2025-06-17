using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        List<BlocoMemoria> memoria = new List<BlocoMemoria>();
        int enderecoAtual = 0;

        Console.WriteLine("Configuração Inicial da Memória:");
        Console.WriteLine("Digite os tamanhos dos blocos de memória separados por espaço:");

        string[] entradasBlocos = Console.ReadLine().Split(' ');
        foreach (var entrada in entradasBlocos)
        {
            if (int.TryParse(entrada, out int tamanhoBloco))
            {
                memoria.Add(new BlocoMemoria
                {
                    Inicio = enderecoAtual,
                    Tamanho = tamanhoBloco,
                    Ocupado = false
                });

                enderecoAtual += tamanhoBloco;
            }
        }

        Console.WriteLine("\nDigite os tamanhos dos processos separados por espaço:");
        string[] entradas = Console.ReadLine().Split(' ');
        List<int> processos = new List<int>();
        foreach (var entrada in entradas)
        {
            if (int.TryParse(entrada, out int tamanho))
                processos.Add(tamanho);
        }

        Console.WriteLine("\nEscolha a estratégia de alocação:");
        Console.WriteLine("1 - First Fit");
        Console.WriteLine("2 - Best Fit");
        int opcao = int.Parse(Console.ReadLine());

        foreach (var processo in processos)
        {
            bool alocado = false;

            if (opcao == 1) // First Fit
            {
                foreach (var bloco in memoria)
                {
                    if (!bloco.Ocupado && bloco.Tamanho >= processo)
                    {
                        Alocar(bloco, processo, memoria);
                        alocado = true;
                        break;
                    }
                }
            }
            else if (opcao == 2) // Best Fit
            {
                BlocoMemoria melhorBloco = null;
                foreach (var bloco in memoria)
                {
                    if (!bloco.Ocupado && bloco.Tamanho >= processo)
                    {
                        if (melhorBloco == null || bloco.Tamanho < melhorBloco.Tamanho)
                            melhorBloco = bloco;
                    }
                }

                if (melhorBloco != null)
                {
                    Alocar(melhorBloco, processo, memoria);
                    alocado = true;
                }
            }

            if (!alocado)
                Console.WriteLine($"Processo de tamanho {processo} não alocado (memória insuficiente).");
        }

        Console.WriteLine("\nEstado final da memória");
        foreach (var bloco in memoria)
            Console.WriteLine(bloco);
    }

    static void Alocar(BlocoMemoria bloco, int tamanhoProcesso, List<BlocoMemoria> memoria)
    {
        int index = memoria.IndexOf(bloco);
        BlocoMemoria novoBloco = new BlocoMemoria
        {
            Inicio = bloco.Inicio,
            Tamanho = tamanhoProcesso,
            Ocupado = true,
            Processo = $"P{tamanhoProcesso}"
        };

        bloco.Inicio += tamanhoProcesso;
        bloco.Tamanho -= tamanhoProcesso;

        if (bloco.Tamanho == 0)
            memoria[index] = novoBloco;
        else
            memoria.Insert(index, novoBloco);
    }
}

class BlocoMemoria
{
    public int Inicio { get; set; }
    public int Tamanho { get; set; }
    public bool Ocupado { get; set; }
    public string Processo { get; set; } = "";

    public override string ToString()
    {
        return $"Início: {Inicio}, Tamanho: {Tamanho}, Ocupado: {Ocupado}, Processo: {Processo}";
    }
}
