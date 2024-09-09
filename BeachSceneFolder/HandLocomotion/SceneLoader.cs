using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject expScreen;
    [SerializeField] private GameObject loadingScreen; // Atribua isso pelo Inspector
    [SerializeField] private Image progressBar; // Se você estiver usando uma barra de progresso
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField]
    private float tempoMinimoDeCarregamento = 2f;
    private bool cenaProntaParaAtivar = false;
    private AsyncOperation operacaoDeCarregamento;

    public void LoadScene(int sceneIndex)
    {
        expScreen.SetActive(false);
        loadingScreen.SetActive(true); // Ativa a tela de loading
        progressBar.fillAmount = 0f;
        progressText.text = "0%";
        StartCoroutine(SimularProgressoDeCarregamento(sceneIndex));
       
    }
    private IEnumerator SimularProgressoDeCarregamento(int index)
    {
        yield return new WaitForSeconds(1); // Pausa antes de iniciar o carregamento, se necessário

        // Inicia o carregamento da cena de forma assíncrona, mas não permite ativação automática
        operacaoDeCarregamento = SceneManager.LoadSceneAsync(index);
        operacaoDeCarregamento.allowSceneActivation = false;

        float tempoPassado = 0f;
        while (tempoPassado < tempoMinimoDeCarregamento)
        {
            tempoPassado += Time.deltaTime;
            float progressoSimulado = tempoPassado / tempoMinimoDeCarregamento;

            // Atualiza a barra de progresso e texto com o progresso simulado
            progressBar.fillAmount = progressoSimulado;
            progressText.text = Mathf.RoundToInt(progressoSimulado * 100) + "%";

            // Aguarda o próximo frame
            yield return null;
        }

        // Espera até que o carregamento real alcance 90% (pronto para ativação)
        while (operacaoDeCarregamento.progress < 0.9f)
        {
            yield return null;
        }

        // Simulação concluída, permite a ativação da cena
        progressText.text = "100%";
        progressBar.fillAmount = 1f;

        // A cena pode ser ativada aqui diretamente ou esperar por uma ação do usuário (e.g., pressionar um botão "Continuar")
        // Para ativar a cena imediatamente após a simulação, remova o comentário da próxima linha:
        AtivarCenaCarregada();
    }

    public void AtivarCenaCarregada()
    {
        if (operacaoDeCarregamento != null)
        {
            operacaoDeCarregamento.allowSceneActivation = true;
        }
    }
}


