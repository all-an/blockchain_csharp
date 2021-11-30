
# Blockchain Simples em C#

## _‘’Chancellor on Brink of Second Bailout for Banks’’ (satoshi on the bitcoin genesis block)_

### Projeto simples porém funcional e interessante para estudos.

## Aprendizado:

> Mais sobre Orientação a Objetos e Generics.
> 
> API Cryptography do .NET
> 
> MemoryStream (super interessante aprender mais sobre fluxo de memória como objeto)
> 
> BinaryWriter (otimização utilizando uma "conversão" de tipos primitivos para binário, ainda estou aprendendo mais sobre o tema)
> 
> E as funcionalidades de uma blockchain em geral

> Nota: `Para executar basta clonar e executar no Visual Studio`  

Bibliotecas utilizadas:

- System.Security.Cryptography

## Caracteristicas

- MemoryStream 
- Hash criptografado
- BinaryWriter
- Geração de blocos

## Método gerador do Hash
```cs
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
```

## Validando bloco
```cs
public static bool ValidarBlocoAnterior(this IBloco bloco, IBloco blocoAnterior) 
        {
            if (blocoAnterior == null) throw new ArgumentNullException(nameof(blocoAnterior));

            var anterior = blocoAnterior.GeradorDoHash();
            return blocoAnterior.ValidarBlocoAtual() && bloco.HashAnterior.SequenceEqual(anterior);  
        }
```

## Método minerador

```cs
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
```

```

| Muito Obrigado | |
| ------ | ------ |
| Linkedin | [https://www.linkedin.com/in/all-an/] |

## Development

| IDE | Visual Studio 2019 Community |
| ------ | ------ |
| .NET | C# | |




#### Building

For production release:

```cs
dotnet
```


## License

MIT

**Free Software! **
