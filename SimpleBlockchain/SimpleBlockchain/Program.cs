using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleBlockchain
{
    public interface IBloco 
    {
        byte[] Dados { get; } //Dados a serem enviados no bloco
        byte[] Hash { get; set; } //identificação e "chave" do bloco
        int Nonce { get; set; } //será um número aleatório para identificação usado uma só vez, google it
        byte[] HashAnterior { get; set; } //hash do bloco anterior
        DateTime TimeStamp { get; } //marcação de data e hora no bloco

    }

    public class Bloco : IBloco
    {
        public Bloco(byte[] dados)
        {
            Dados = dados ?? throw new ArgumentNullException(nameof(dados));
            Nonce = 0;
            HashAnterior = new byte[] { 0x00 };
            TimeStamp = DateTime.Now;
        }
        public byte[] Dados { get; }
        public byte[] Hash { get; set; }
        public int Nonce { get; set; } // Nonce é abreviação para "number only used once" número usado uma só vez
                                       // Este número é o usado pela mineração para alcançar a dificuldade
        public DateTime TimeStamp { get; }
        public byte[] HashAnterior { get; set; }

        public override string ToString()
        {
            //formatando o hash para imprimir na tela
            return $"{BitConverter.ToString(Hash).Replace("-", "")}:\n{BitConverter.ToString(HashAnterior).Replace("-", "")}\n {Nonce} {TimeStamp}";
        }
    }

    public static class FuncionalidadesDaBlockChain
    {
        public static byte[] GeradorDoHash(this IBloco bloco) //método gerador do hash 
        {
            
            using (SHA512 sha = new SHA512Managed())  //criando um objeto do tipo Hash SHA512 usando a biblioteca Cryptography
            using (MemoryStream stream = new MemoryStream()) //criando um objeto do tipo Fluxo de Memória
            using (BinaryWriter binaryWriter = new BinaryWriter(stream)) // passando o objeto Fluxo de Memória para o objeto Binary Writer
            {
                //escrevendo e atribuindo o fluxo de memória as propriedades do bloco
                binaryWriter.Write(bloco.Dados);
                binaryWriter.Write(bloco.Nonce);
                binaryWriter.Write(bloco.TimeStamp.ToBinary());
                binaryWriter.Write(bloco.HashAnterior);
                var arrayStream = stream.ToArray();
                return sha.ComputeHash(arrayStream); // criptografando o fluxo de memória
            }

        }
        public static byte[] MineHash(this IBloco bloco, byte[] dificuldade) //método para mineração do bloco 
        {
            if (dificuldade == null) throw new ArgumentNullException(nameof(dificuldade));

            byte[] hash = new byte[0];
            int d = dificuldade.Length;
            //preenchendo o hash 
            while (!hash.Take(2).SequenceEqual(dificuldade))
            {
                bloco.Nonce++;
                hash = bloco.GeradorDoHash();
            }
            return hash;
        } 
        public static bool ValidarBlocoAtual(this IBloco bloco) // validando o bloco 
        {
            var blc = bloco.GeradorDoHash();
            
            return bloco.Hash.SequenceEqual(blc); //retorna o bloco gerado
        }
        public static bool ValidarBlocoAnterior(this IBloco bloco, IBloco blocoAnterior) 
        {
            if (blocoAnterior == null) throw new ArgumentNullException(nameof(blocoAnterior));

            var anterior = blocoAnterior.GeradorDoHash();
            return blocoAnterior.ValidarBlocoAtual() && bloco.HashAnterior.SequenceEqual(anterior);  
        }
        public static bool ValidarCadeia(this IEnumerable<IBloco> itens) 
        {
            Console.WriteLine("Bloco Válido");
            var enumerable = itens.ToList();
            return enumerable.Zip(enumerable.Skip(1), Tuple.Create).All(bloco => bloco.Item2.ValidarBlocoAtual() && bloco.Item2.ValidarBlocoAnterior(bloco.Item1));
        }
    }

    public class BlockChain : IEnumerable<IBloco>
    {
        public List<IBloco> Itens //propriedade
        {
            get => _itens;
            set => _itens = value;
        }

        public byte[] Dificuldade { get; }

        private List<IBloco> _itens = new List<IBloco>(); //referenciando um objeto do tipo Lista IBloco

        public BlockChain(byte[] dificuldade, IBloco genesis) //construtor do bloco
        {
            Dificuldade = dificuldade;  //medida de dificuldade para decodificação do bloco
            genesis.Hash = genesis.MineHash(dificuldade);
            Itens.Add(genesis);
        }
        public void Add(IBloco item) //adicionando item ao bloco
        {
            if (Itens.LastOrDefault() != null)
            {
                item.HashAnterior = Itens.LastOrDefault()?.Hash; //retorna o último item do bloco (ou o padrão caso não haja item) ou o Hash
            }
            item.Hash = item.MineHash(Dificuldade);
            Itens.Add(item);
        }

        public int Count => Itens.Count; //contador para a alteração dos itens do bloco

        public IBloco this[int index]
        {
            get => Itens[index];
            set => Itens[index] = value;
        }

        public IEnumerator<IBloco> GetEnumerator()
        {
            return Itens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Itens.GetEnumerator();
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            Random rnd = new Random(DateTime.UtcNow.Millisecond);

            IBloco genesis = new Bloco(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 });

            byte[] dificuldade = new byte[] { 0x00, 0x00 };

            BlockChain chain = new BlockChain(dificuldade, genesis);

            for(int i = 0; i < 200; i++)
            {
                var dados = Enumerable.Range(0, 2256).Select(p => (byte)rnd.Next());
                chain.Add(new Bloco(dados.ToArray()));

                Console.WriteLine(chain.LastOrDefault() ? .ToString());

                Console.WriteLine($"BlockChain é Válida: {chain.ValidarCadeia()}");
            }

            Console.ReadLine();    
        }
    }
}