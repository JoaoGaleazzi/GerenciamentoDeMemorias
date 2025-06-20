using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        while (true)
        {
            List<BlocoMemoria> memoriaLivre = new List<BlocoMemoria>();
            List<Alocacao> processosAlocados = new List<Alocacao>();
            List<BlocoOriginal> blocosOriginais = new List<BlocoOriginal>();
            List<(int Inicio, int Fim)> espacosSeguranca = new List<(int, int)>();
            int enderecoAtual = 0;

            Console.WriteLine("\nSimulador de Alocação de Memória\n");

            Console.WriteLine("Digite os tamanhos dos blocos de memória (separados por espaço):");
            string[] entradasBlocos = Console.ReadLine().Split(' ');

            foreach (var entrada in entradasBlocos)
            {
                if (int.TryParse(entrada, out int tamanho))
                {
                    memoriaLivre.Add(new BlocoMemoria { Inicio = enderecoAtual, Tamanho = tamanho });
                    blocosOriginais.Add(new BlocoOriginal { Inicio = enderecoAtual, Fim = enderecoAtual + tamanho - 1 });
                    enderecoAtual += tamanho;
                }
            }

            int espacoSeguranca;
            while (true)
            {
                Console.WriteLine("\nDigite o espaço de segurança entre processos (em unidades de endereço):");
                if (int.TryParse(Console.ReadLine(), out espacoSeguranca) && espacoSeguranca >= 0)
                    break;
                else
                    Console.WriteLine("Valor inválido. Digite um número inteiro maior ou igual a 0.");
            }

            Console.WriteLine("\nDigite os tamanhos dos processos (separados por espaço):");
            string[] entradasProcessos = Console.ReadLine().Split(' ');
            List<int> processos = new List<int>();
            foreach (var entrada in entradasProcessos)
            {
                if (int.TryParse(entrada, out int tamanho))
                    processos.Add(tamanho);
            }

            Console.WriteLine("\nEscolha a estratégia de alocação:");
            Console.WriteLine("1 - First Fit");
            Console.WriteLine("2 - Best Fit");
            Console.WriteLine("3 - Worst Fit");
            int opcao = int.Parse(Console.ReadLine());

            foreach (var processo in processos)
            {
                BlocoMemoria blocoEscolhido = null;

                if (opcao == 1) // First
                {
                    foreach (var bloco in memoriaLivre)
                    {
                        if (bloco.Tamanho >= processo + espacoSeguranca)
                        {
                            blocoEscolhido = bloco;
                            break;
                        }
                    }
                }
                else if (opcao == 2) // Best
                {
                    foreach (var bloco in memoriaLivre)
                    {
                        if (bloco.Tamanho >= processo + espacoSeguranca)
                        {
                            if (blocoEscolhido == null || bloco.Tamanho < blocoEscolhido.Tamanho)
                                blocoEscolhido = bloco;
                        }
                    }
                }
                else if (opcao == 3) // Worst
                {
                    foreach (var bloco in memoriaLivre)
                    {
                        if (bloco.Tamanho >= processo + espacoSeguranca)
                        {
                            if (blocoEscolhido == null || bloco.Tamanho > blocoEscolhido.Tamanho)
                                blocoEscolhido = bloco;
                        }
                    }
                }

                if (blocoEscolhido != null)
                {
                    processosAlocados.Add(new Alocacao
                    {
                        Processo = $"P{processo}",
                        Inicio = blocoEscolhido.Inicio,
                        Tamanho = processo
                    });

                    if (espacoSeguranca > 0)
                    {
                        espacosSeguranca.Add((
                            blocoEscolhido.Inicio + processo,
                            blocoEscolhido.Inicio + processo + espacoSeguranca - 1
                        ));
                    }

                    blocoEscolhido.Inicio += processo + espacoSeguranca;
                    blocoEscolhido.Tamanho -= processo + espacoSeguranca;

                    if (blocoEscolhido.Tamanho <= 0)
                        memoriaLivre.Remove(blocoEscolhido);
                }
                else
                {
                    Console.WriteLine($"Processo de tamanho {processo} não alocado (memória insuficiente).");
                }
            }

            Console.WriteLine("\nEstrutura inicial dos Blocos de Memória (inicial)");
            int contadorBloco = 1;
            foreach (var bloco in blocosOriginais)
            {
                Console.WriteLine($"Bloco {contadorBloco}: [{bloco.Inicio}-{bloco.Fim}]");
                contadorBloco++;
            }

            Console.WriteLine("\nMapa completo da memória");

            foreach (var blocoOriginal in blocosOriginais)
            {
                Console.WriteLine($"\nBloco [{blocoOriginal.Inicio}-{blocoOriginal.Fim}]:");

                int pos = blocoOriginal.Inicio;
                while (pos <= blocoOriginal.Fim)
                {
                    bool encontrado = false;

                    foreach (var aloc in processosAlocados)
                    {
                        if (pos == aloc.Inicio)
                        {
                            int fim = aloc.Inicio + aloc.Tamanho - 1;
                            Console.WriteLine($"  [{aloc.Inicio}-{fim}]: {aloc.Processo}");
                            pos = fim + 1;
                            encontrado = true;
                            break;
                        }
                    }

                    if (encontrado) continue;

                    foreach (var espaco in espacosSeguranca)
                    {
                        if (pos >= espaco.Inicio && pos <= espaco.Fim)
                        {
                            Console.WriteLine($"  [{espaco.Inicio}-{espaco.Fim}]: Espaço de Segurança");
                            pos = espaco.Fim + 1;
                            encontrado = true;
                            break;
                        }
                    }

                    if (encontrado) continue;

                    foreach (var livre in memoriaLivre)
                    {
                        if (pos == livre.Inicio)
                        {
                            int fim = livre.Inicio + livre.Tamanho - 1;
                            Console.WriteLine($"  [{livre.Inicio}-{fim}]: Livre");
                            pos = fim + 1;
                            encontrado = true;
                            break;
                        }
                    }

                    if (!encontrado)
                    {
                        Console.WriteLine($"  [{pos}]: Indefinido");
                        pos++;
                    }
                }
            }

            Console.WriteLine("\nDeseja fazer outra simulação? (s/n)");
            string resposta = Console.ReadLine().ToLower();

            if (resposta != "s")
            {
                break;
            }

            Console.Clear();
        }
    }
}

class BlocoMemoria
{
    public int Inicio { get; set; }
    public int Tamanho { get; set; }
}

class Alocacao
{
    public string Processo { get; set; }
    public int Inicio { get; set; }
    public int Tamanho { get; set; }
}

class BlocoOriginal
{
    public int Inicio { get; set; }
    public int Fim { get; set; }
}
