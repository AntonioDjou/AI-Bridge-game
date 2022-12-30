using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambiente : MonoBehaviour {

    public Piao[] piaos;

    public List<Piao> piaosLeft = new List<Piao>();
    public List<Piao> piaosRight = new List<Piao>();

    public int idxSequencia = 0;

    public List<ThinkEnv> sequencia;

    // Use this for initialization
    void Start () {
        foreach (Piao piao in piaos) {
            piaosLeft.Add(piao);
        }

        ThinkEnv think = new ThinkEnv(piaosLeft, piaosRight, 0, null, new Action(), true);
        Resolver agente = new Resolver(think);

        agente.menorCusto();

        sequencia = agente.sequencia();
    }
	
	// Update is called once per frame
	void Update () {
        // se tiver fora da sequencia, nem adianta
        if (idxSequencia >= sequencia.Count) {
            return;
        }

        ThinkEnv atual = sequencia[idxSequencia];
        Debug.Log("idx sequencia " + idxSequencia);
        Debug.Log("atual.acaoTransformacao.type " + atual.acaoTransformacao.type);

        // para o caso de inicial, vai para o próximo passo na sequência
        if (atual.acaoTransformacao.type == 0) {
            idxSequencia++;
        } else {
            // detectar se tem peça em movimento
            foreach (Piao piao in piaos) {
                if (piao.move != 0) {
                    // ainda há movimento, não precisa fazer nada
                    return;
                }
            }
            // não detectou movimento, aciona o estado atual e vai para o próximo na sequência
            Action acao = atual.acaoTransformacao;

            if (acao.type == +1) {
                move_2_lr(acao.p1, acao.p2);
            } else {
                move_1_rl(acao.p1);
            }
            idxSequencia++;
        }
	}

    void move_2_lr(Piao p1, Piao p2) {
        int lazy = Mathf.Max(p1.laziness, p2.laziness);
        Debug.Log("movendo os objetos " + p1.ToString() + " e " + p2.ToString() + " com laziness de " + lazy);
        p1.move_to(+1, lazy);
        p2.move_to(+1, lazy);
    }

    void move_1_rl(Piao p1) {
        p1.move_to(-1, p1.laziness);
    }
}
