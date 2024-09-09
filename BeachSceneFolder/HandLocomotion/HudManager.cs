using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    [Header("Screens to build Tree")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject tutorialQuestionScreen;
    [SerializeField] private GameObject[] tutorialScreens;
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject returnScreen;

    [Header("Feedbacks References")]
    [SerializeField] private GameObject _checkMarkAnimation;
    [SerializeField] private GameObject _checkMark2;
    [SerializeField] private GameObject _thumbsUpAnimation;
    [SerializeField] private GameObject _passPokeButton;
    [SerializeField] private GameObject _canvasObject;
    [SerializeField] private GameObject _canvasMain;
    [SerializeField] private Toggle[] toggles;
    [SerializeField] private Color lightGreen;


    private ScreenNode root;
    private PoseDetectionActor detective;
    public Vector3 relativePosition; // Posição relativa desejada em relação à câmera
    public float fixedYPosition; // A posição Y fixa que você quer manter para o CurvedUnityCanvas
    [SerializeField] private float rotationOffset = 17f;

    public bool isOnTutorial, isOnThumbsTutorial, isOnPokeTutorial, isOnUITutorial, isOnFinalTutorial;
    void Start()
    {
        detective = GetComponent<PoseDetectionActor>();
        _passPokeButton.SetActive(false);
        isOnTutorial = false;
        isOnThumbsTutorial = false;
        isOnPokeTutorial = false;
        isOnUITutorial = false;
        isOnFinalTutorial = false;
        PositionCanvas();
        BuildScreenTree(); // build tree
        NavigateTo(root); // Start navegation
    }

    void BuildScreenTree()
    {
        root = new ScreenNode(startScreen);
        ScreenNode tutorialQuestion = new ScreenNode(tutorialQuestionScreen);
        root.AddChild(tutorialQuestion,root); //append first child

        //append all tutorial childs
        ScreenNode lastTutorialScreen = tutorialQuestion; // set the first tutorial screen to be child of tutorial question 
        //loop trought all tutorial screen, and keep appending them to the previous tutorial
        foreach (GameObject tutorialScreen in tutorialScreens)
        {
            ScreenNode newScreenNode = new ScreenNode(tutorialScreen);
            lastTutorialScreen.AddChild(newScreenNode,lastTutorialScreen);
            lastTutorialScreen = newScreenNode;
        }
        
        //connect the hole tree to the main menu
        ScreenNode mainMenu = new ScreenNode(mainMenuScreen);
        ScreenNode mainMenuEnd = new ScreenNode(mainMenuScreen);
        ScreenNode returnScreenNode = new ScreenNode(returnScreen);

        lastTutorialScreen.AddChild(mainMenuEnd,lastTutorialScreen); //Conecta o fim do tutorial ao menu principal
        tutorialQuestion.AddChild(mainMenu,tutorialQuestion); //Conecta a opção de pular o tutorial ao menu principal   
        mainMenu.AddChild(returnScreenNode, mainMenu);
        mainMenuEnd.AddChild(returnScreenNode, mainMenu);
    }

    public void NavigateTo(ScreenNode targetScreen)
    {   
        //in each transition change the opacity of the role gameobject, or call animation
        //
        //Debug.Log(targetScreen.Screen.name + " //// " + targetScreen.Parent.name);
        if(targetScreen.Screen != root.Screen)
        {
            if (targetScreen.Parent.Screen != null)
            {
                Debug.Log("Desativando tela passada: " + targetScreen.Parent.Screen.name);
                targetScreen.Parent.Screen.SetActive(false);
            }
            else
            {
                Debug.Log("Parent Screen is not defined, check the inspector!");
            }
        }
        else
        {
            Debug.Log("First root node");

        }
        targetScreen.Screen.SetActive(true);
    }
    public void GoBack(ScreenNode targetScreen)
    {
        //in each transition change the opacity of the role gameobject, or call animation
        //
        //Debug.Log(targetScreen.Screen.name + " //// " + targetScreen.Parent.name);
        if (targetScreen.Screen != root.Screen)
        {
            if (targetScreen.Children != null && targetScreen.Children.Count > 0)
            {
                foreach (var child in targetScreen.Children)
                {
                    Debug.Log("Desativando tela: " + child.Screen.name);
                    child.Screen.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Nenhum filho para desativar ou verificar no inspetor!");
            }
        }
        else
        {
            Debug.Log("First root node");

        }
        targetScreen.Screen.SetActive(true);
    }

    public void PositionCanvas()
    {
        Vector3 newPosition = detective.camera.transform.position + detective.camera.transform.TransformDirection(new Vector3(relativePosition.x, 0, relativePosition.z));
        newPosition.y = fixedYPosition; // Sobrescreve a posição Y

        //Atualiza a posição do CurvedUnityCanvas diretamente
        _canvasMain.transform.position = newPosition;

        //Ajusta a rotação para olhar para a câmera, opcionalmente mantendo o eixo Y fixo na rotação também
        Vector3 lookDirection = new Vector3(detective.camera.transform.position.x, _canvasMain.transform.position.y, detective.camera.transform.position.z) - _canvasMain.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(-lookDirection);
        //_canvasMain.transform.rotation = Quaternion.LookRotation(-lookDirection);

        Quaternion xRotation = Quaternion.Euler(rotationOffset, 0, 0);
        _canvasMain.transform.rotation = lookRotation * xRotation;
    }
    //
    //every screen path begins with the root node, that way, u dont need to create a both way road system
    #region Navegation
    public void GoToTutorialQuestion()
    {
        Debug.Log("Tutorial Node: " + root.Children[0].Screen.name);
        NavigateTo(root.Children[0]);
    }

    public void StartTutorial()
    {
        isOnTutorial = true; isOnThumbsTutorial = true;
        NavigateTo(root.Children[0].Children[0]);
    }

    public void SkipTutorial()
    {
        isOnTutorial = false;
        NavigateTo(root.Children[0].Children[1]);
    }
    public void InitSecondTutorial()
    {
        //second tuto
        isOnThumbsTutorial = false; isOnPokeTutorial = true;
        NavigateTo(root.Children[0].Children[0].Children[0]);
    }
    public void InitThirdTutorial()
    {
        //third tuto
        isOnPokeTutorial = false; isOnUITutorial = true;
        NavigateTo(root.Children[0].Children[0].Children[0].Children[0]);
    }
    public void InitFourthTutorial()
    {
        //fourth tuto
        isOnUITutorial = false; isOnFinalTutorial = true;
        NavigateTo(root.Children[0].Children[0].Children[0].Children[0].Children[0]);
    }
    public void EndTutorial()
    {
        isOnTutorial = false; isOnFinalTutorial = false;
        
        Debug.Log("Tutorial Node ultimo parent: " + root.Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].Parent.Screen.name);
        NavigateTo(root.Children[0].Children[0].Children[0].Children[0].Children[0].Children[0]);
    }
    public void GoToReturnScreen()
    {
        NavigateTo(root.Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].Children[0]);
    }
    public void ReturnButton()
    {
        detective.TeleportPlayerBack();
        GoBack(root.Children[0].Children[0].Children[0].Children[0].Children[0].Children[0]);
    }
    #endregion

    #region Animations&Feedback
    public void SetThumbsTutoAnimation(bool _markState)
    {
        _checkMarkAnimation.GetComponent<Animator>().SetBool("canPassMark", _markState);
        _thumbsUpAnimation.GetComponent<Animator>().SetBool("canPassThumbs", _markState);
    }

    int count = 0;
    public void PokeTutorial(Toggle t)
    {
        Debug.Log("Valor de Count: " + count);
        bool value = t.isOn;
        Debug.Log("Valor de Value: " + value);
        if (value)
        {
            t.gameObject.GetComponent<Image>().color = lightGreen;
            if (count < 2)
            {
                count = count + 1;
            }
            else
            {
                //enable continue button
                for(int i=0;i< toggles.Length; i++)
                {
                    toggles[i].interactable = false;
                }
                _passPokeButton.SetActive(true);
            }
        }
        else
        {
            t.gameObject.GetComponent<Image>().color = Color.white;
            if (count > 0)
                count--;
        }
        
     
    }

    public void PassPokeTutorial()
    {
        InitThirdTutorial();
    }
    
    public void DealWithMainUI(bool _value)
    {
        _canvasObject.GetComponent<Animator>().SetBool("turnOffUI", _value);   
    }

    public bool wasDisabled = false, wasEnabled = false;
    public void UITutorial()
    {
        if(wasDisabled && wasEnabled)
        {
            //check mark // setbool
            _checkMark2.GetComponent<Animator>().SetBool("canPassMark2", true);
        }
        else
        {
            _checkMark2.GetComponent<Animator>().SetBool("canPassMark2", false);
        }

    }
    public void InitCountDown()
    {
        StartCoroutine("OffUITuto");
    }
    public void KillCountDown()
    {
        StopCoroutine("OffUITuto");
    }
    private IEnumerator OffUITuto()
    {
        yield return new WaitForSecondsRealtime(5f);
        wasDisabled = false;
        DealWithMainUI(false);
    }

    public void JumpToPosition(GameObject triggerObj)
    {
        detective.TeleportPlayer(triggerObj);
        GoToReturnScreen();
    }
 
    #endregion
}
