using System;
using System.Numerics;

namespace LabRats
{
    class Program
    {
        static void Main(string[] args)
        {
            GeradorArquivo.GerarArquivo();
            Console.Clear();
            Labirinto lab = new Labirinto();
            Lista lista = new Lista();
            Rato rato = new Rato(lab.ratoPos, lab.ratoPos, lab.ratoPos);
            lista.InsereFim(lab.ratoPos);
            lab.Mostrar();
            Console.ReadKey();
            Console.Clear();
            rato.ProcurarCaminhos(lab.lab);
            (lab.lab, lista) = rato.Caminhar(lab.lab, rato.possiblePos, lista);
            lab.Mostrar();
            Console.ReadKey();
        }
    }
    public class Labirinto
    {
        public char[,] lab;
        public Vector2 ratoPos;
        public Vector2 queijoPos;
        public int lengthX;
        public int lengthY;

        public Labirinto()
        {
            lab = LeDeArquivo(GeradorArquivo.archiveName);
        }
        public void Mostrar()
        {
            for (int i = 0; i < lengthX; i++)
            {
                for (int j = 0; j < lengthY; j++)
                {
                    Console.Write($"{lab[i, j]}");
                }
                Console.WriteLine();
            }
        }
        public char[,] LeDeArquivo(string caminho)
        {
            string linha;
            string[] campos;

            using (StreamReader sw = new StreamReader(caminho))
            {
                linha = sw.ReadLine();

                campos = linha.Split(' ');

                lengthX = int.Parse(campos[0]);
                lengthY = int.Parse(campos[1]);

                linha = sw.ReadLine();

                campos = linha.Split(' ');

                ratoPos = new Vector2(Convert.ToInt32(campos[1]), Convert.ToInt32(campos[0]));

                linha = sw.ReadLine();

                campos = linha.Split(' ');

                queijoPos = new Vector2(Convert.ToInt32(campos[1]), Convert.ToInt32(campos[0]));

                char[,] labirinto = new char[lengthX, lengthY];

                linha = sw.ReadLine();
                for (int i = 0; i < lengthX; i++)
                {
                    for (int j = 0; j < lengthY; j++)
                    {
                        labirinto[i, j] = linha[j];
                    }
                    linha = sw.ReadLine();
                }
                labirinto[(int)ratoPos.X, (int)ratoPos.Y] = 'R';
                labirinto[(int)queijoPos.X, (int)queijoPos.Y] = 'Q';
                return labirinto;
            }
        }
    }
    public class Rato
    {
        Vector2 pos;
        Vector2 nextPos;
        Vector2 pastPos;

        public List<Vector2> possiblePos;

        public Vector2[] directions = {
            new Vector2(1,0), //direita
            new Vector2(-1,0), //esquerda
            new Vector2(0,1), //baixo
            new Vector2(0,-1) //cima
        };

        public Rato(Vector2 _pos, Vector2 _nextPos, Vector2 _pastPos)
        {
            pos = _pos;
            nextPos = _nextPos;
            pastPos = _pastPos;
        }

        public void ProcurarCaminhos(char[,] lab)
        {
            Vector2 position;
            List<Vector2> possiblePositions = new List<Vector2>();

            for (int i = 0; i < directions.Length; i++)
            {
                position = pos + directions[i];
                if ((lab[(int)position.X, (int)position.Y] == '0'))//tem caminho
                {
                    //guardar caminho possivel
                    possiblePositions.Add(position);
                    if ((lab[(int)position.X, (int)position.Y] == 'Q'))
                    {
                        possiblePositions.Clear();
                        possiblePositions.Add(position);
                        break;
                    }
                }
            }
            possiblePos = possiblePositions;
        }
        public (char[,], Lista) Caminhar(char[,] lab, List<Vector2> positions, Lista lista)
        {
            if (positions.Count() > 0)
            {
                if (lab[(int)positions[0].X, (int)positions[0].Y] == 'Q')
                {
                    //Chegou ao queijo
                    lista.InsereFim(positions[0]);
                    return (lab, lista);
                }
                else
                {
                    pastPos = pos;
                    nextPos = positions[0];
                    lab[(int)pos.X, (int)pos.Y] = '0';
                    lab[(int)nextPos.X, (int)nextPos.Y] = 'R';
                    lista.InsereFim(positions[0]);
                }

            }
            return (lab, lista);
        }
    }
    public class Nodo
    {
        public Vector2 elemento;
        public Nodo seguinte; // ponteiro

        public Nodo(Vector2 _elemento, Nodo seguinte)
        {
            this.elemento = _elemento;
            this.seguinte = seguinte;
        }
    }
    public class Lista
    {
        public Nodo primeiro { get; set; }// Cabeça ou Head
        public Nodo ultimo { get; set; }

        public Lista()
        {
            primeiro = new Nodo(new Vector2(0,0), null);
            ultimo = primeiro;
        }

        public void InsereFim(Vector2 elemento)
        {
            Nodo nodo = new Nodo(elemento, null);
            ultimo.seguinte = nodo;
            ultimo = nodo;
        }

        public Vector2 Pesquisar(Vector2 elemento)
        {
            Nodo nodo = primeiro.seguinte;
            Vector2 _elemento = new Vector2(0,0);
            while (nodo != null)
            {
                if (nodo.elemento == elemento)
                {
                    _elemento = nodo.elemento;
                    break;
                }
                nodo = nodo.seguinte;
            }
            return _elemento;
        }
        

        public Vector2 RemoveInicio()
        {
            if (primeiro.seguinte == null)
            {
                throw new Exception("Erro: Lista está vázia!");
            }

            Nodo nodo = primeiro.seguinte;
            primeiro.seguinte = nodo.seguinte;

            if (primeiro.seguinte == null)
            {
                ultimo = primeiro;
            }
            return nodo.elemento;
        }

        public Vector2 RemoveFim()
        {
            Vector2 vec2 = ultimo.elemento;
            if (primeiro == ultimo)
            {
                throw new Exception("Erro: Lista está vázia!");
            }

            Nodo auxiliar = primeiro;
            while (auxiliar.seguinte != ultimo)
            {
                auxiliar = auxiliar.seguinte;
            }
            ultimo = auxiliar;
            ultimo.seguinte = null;

            return vec2;
        }

        public void Mostrar()
        {
            Nodo nodo = primeiro.seguinte;

            Console.WriteLine("{");

            while (nodo != null)
            {
                Console.Write($"\t({nodo.elemento.X.ToString()}, {nodo.elemento.Y.ToString()})\n");

                nodo = nodo.seguinte;
            }

            Console.WriteLine("}");
        }
        public int Quantidade()
        {
            int qtd = 0;
            Nodo nodo = primeiro.seguinte;
            while (nodo != null)
            {
                qtd++;
                nodo = nodo.seguinte;
            }
            return qtd;
        }
        public void GravaEmArquivo(string caminho)
        {
            Nodo nodo = primeiro.seguinte;
            int qtd = Quantidade();

            using (StreamWriter sw = new StreamWriter(caminho))
            {
                sw.WriteLine(qtd.ToString());
                while (nodo != null)
                {
                    Console.Write($" ({nodo.elemento.X.ToString()}, {nodo.elemento.Y.ToString()})");

                    nodo = nodo.seguinte;
                }
            }
        }
    }
}

