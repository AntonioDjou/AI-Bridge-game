using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinkEnv {
    public List<Piao> pessoasEsquerdo;
    public List<Piao> pessoasDireito;
    public int custoTotal;
    public ThinkEnv estadoPai;
    public Action acaoTransformacao;
    public bool lanternaEsquerda;

    public ThinkEnv(List<Piao> pessoasEsquerdo, List<Piao> pessoasDireito, int custoTotal, ThinkEnv estadoPai, Action acaoTransformacao, bool lanternaEsquerda) {
        this.pessoasEsquerdo = pessoasEsquerdo;
        this.pessoasDireito = pessoasDireito;
        this.custoTotal = custoTotal;
        this.estadoPai = estadoPai;
        this.acaoTransformacao = acaoTransformacao;
        this.lanternaEsquerda = lanternaEsquerda;
    }

    public bool fim() {
        return pessoasEsquerdo.Count == 0;
    }

    public ThinkEnv move1personRL(int idx1) {
        List<Piao> novoEsquerdo = new List<Piao>();
        List<Piao> novoDireito = new List<Piao>();

        foreach (Piao piaoEsquerdo in this.pessoasEsquerdo) {
            novoEsquerdo.Add(piaoEsquerdo);
        }
        int idx = 0;
        Piao piaoMovido1 = null;
        foreach (Piao piaoDireito in this.pessoasDireito) {
            if (idx == idx1) {
                novoEsquerdo.Add(piaoDireito);
                piaoMovido1 = piaoDireito;
            } else {
                novoDireito.Add(piaoDireito);
            }
            
            idx++;
        }

        int novoCusto = this.custoTotal + piaoMovido1.laziness;
		
		return new ThinkEnv(novoEsquerdo, novoDireito, novoCusto, this, new Action(piaoMovido1), !this.lanternaEsquerda);
	}

    public ThinkEnv move2personLR(int idx1, int idx2) {
        List<Piao> novoEsquerdo = new List<Piao>();
        List<Piao> novoDireito = new List<Piao>();

        foreach (Piao piaoDireito in this.pessoasDireito) {
            novoDireito.Add(piaoDireito);
        }
        int idx = 0;
        Piao piaoMovido1 = this.pessoasEsquerdo[0];
        Piao piaoMovido2 = this.pessoasEsquerdo[0];
        foreach (Piao piaoEsquerdo in this.pessoasEsquerdo) {
            if (idx == idx1) {
                novoDireito.Add(piaoEsquerdo);
                piaoMovido1 = piaoEsquerdo;
            } else if (idx == idx2) {
                novoDireito.Add(piaoEsquerdo);
                piaoMovido2 = piaoEsquerdo;
            } else {
                novoEsquerdo.Add(piaoEsquerdo);
            }

            idx++;
        }
        int maiorCusto = piaoMovido1.laziness > piaoMovido2.laziness ? piaoMovido1.laziness : piaoMovido2.laziness;
        int novoCusto = this.custoTotal + maiorCusto;

        return new ThinkEnv(novoEsquerdo, novoDireito, novoCusto, this, new Action(piaoMovido1, piaoMovido2), !this.lanternaEsquerda);
    }
}

public class Resolver {
    ThinkEnv inicial;
    int melhorResultado;
    long iteracoesMenor = 0;
    long iteracoes = 0;

    public ThinkEnv melhorEstado = null;

    public Resolver(ThinkEnv inicial) {
        this.inicial = inicial;

        // heurística para um upper cut:
        // 		* ignora a existência do lado direito, já que posso fazer toda a migração sócom gente do esquerdo 
        // 		* repetir até não sobrar ninguém:
        // 		* 		vão as duas pessoas de maior peso
        // 		* 		se tiver algum do outro lado, retornar a pessoa mais leve
        // com isso, temos que:
        // 		* o mais pesado só conta uma vez
        // 		* o mais leve não pe levado em consideração
        // 		* todos os demais são contados duas vezes
        // em outras palavras:
        // 		heurística = soma todos * 2 - maior - 2*menor
        //
        // uma outra heurística:
        // 		* o menor acompanha todos os demais
        // 		* se tiver alguém ainda do lado esquerdo, o menor volta
        // em outras palavras:
        // 		heurística2 = soma todos - menor + (quantidade - 1)*menor
        int somaHeuristica1 = 0;
        int somaHeuristica2 = 0;
        int maior = inicial.pessoasEsquerdo[0].laziness;
        int menor = inicial.pessoasEsquerdo[0].laziness;

        int size = 0;

        foreach (Piao piao in inicial.pessoasEsquerdo) {
            size++;
            int peso = piao.laziness;
            somaHeuristica1 += 2 * peso;
            somaHeuristica2 += peso;
            if (peso > maior) {
                maior = peso;
            } else if (peso < menor) {
                menor = peso;
            }
        }

        size = inicial.pessoasEsquerdo.Count;

        // As somas heurísticas só funcionam se tiver pelo menos 2 pessoas
        if (size >= 2) {
            somaHeuristica1 = somaHeuristica1 - maior - 2 * menor;
            somaHeuristica2 = somaHeuristica2 + (size - 2) * menor;
        } else {
            // se tiver só uma pessoa, o melhor caso é só a soma básica
            somaHeuristica1 = somaHeuristica2;
        }
        
        melhorResultado = (somaHeuristica1 < somaHeuristica2? somaHeuristica1: somaHeuristica2) + 1;
    }

    public void menorCusto() {
        menorCusto(inicial, 0);
    }

    public List<ThinkEnv> sequencia() {
        return sequencia(new List<ThinkEnv>(), melhorEstado);
    }

    private List<ThinkEnv> sequencia(List<ThinkEnv> list, ThinkEnv estadoAnalise) {
        if (estadoAnalise != null) {
            sequencia(list, estadoAnalise.estadoPai);
            list.Add(estadoAnalise);
        }

        return list;
    }

    private void menorCusto(ThinkEnv estadoBase, int level) {
        iteracoes++;
        if (estadoBase.custoTotal >= melhorResultado) {
            return;
        }

        if (estadoBase.fim()) {
            melhorResultado = estadoBase.custoTotal;
            melhorEstado = estadoBase;
            iteracoesMenor = iteracoes;

            Debug.Log(
                    "Achou bom resultado!\n" +
                    "\tmelhorResultado " + melhorResultado + "\n" +
                    "\titeracoesMenor " + iteracoesMenor
                );

            return;
        }

        if (estadoBase.lanternaEsquerda) {
            // todas as possibilidades move 2 LR
            for (int i = 0; i < estadoBase.pessoasEsquerdo.Count - 1; i++) {
                for (int j = i + 1; j < estadoBase.pessoasEsquerdo.Count; j++) {
                    menorCusto(estadoBase.move2personLR(i, j), level + 1);
                }
            }
        } else {
            // todas as possibilidades move 1 RL
            for (int i = 0; i < estadoBase.pessoasDireito.Count; i++) {
                menorCusto(estadoBase.move1personRL(i), level + 1);
            }
        }
    }
}

public class Action {
    public Piao p1, p2;
    public int type;

    // tipo +1 => LR
    public Action(Piao p1, Piao p2) {
        this.p1 = p1;
        this.p2 = p2;

        type = +1;
    }

    // tipo -1 => RL
    public Action(Piao p1) {
        this.p1 = p1;
        this.p2 = null;

        type = -1;
    }

    // tipo 0 => inicio
    public Action() {
        this.p1 = null;
        this.p2 = null;

        type = 0;
    }
}
