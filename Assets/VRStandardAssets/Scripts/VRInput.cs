using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; //added by osharaki

namespace VRStandardAssets.Utils
{
    // This class encapsulates all the input required for most VR games.
    // It has events that can be subscribed to by classes that need specific input.
    // This class must exist in every scene and so can be attached to the main
    // camera for ease.
    public class VRInput : MonoBehaviour
    {
        //Swipe directions
        public enum SwipeDirection
        {
            NONE,
            UP,
            DOWN,
            LEFT,
            RIGHT
        };


        public event Action<SwipeDirection> OnSwipe;                // Called every frame passing in the swipe, including if there is no swipe.
        public event Action OnClick;                                // Called when Fire1 is released and it's not a double click.
        public event Action OnTouchpadHold;    //Added by osharaki. Called when touchpad is pressed and held.
        public event Action OnDown;                                 // Called when Fire1 is pressed.
        public event Action OnUp;                                   // Called when Fire1 is released.
        public event Action OnDoubleClick;                          // Called when a double click is detected.
        public event Action OnCancel;                               // Called when Cancel is pressed.
        public Text myTextbox;  //added by osharaki
        bool alreadyClicked = false; //added by osharaki

        [SerializeField] private float m_DoubleClickTime = 0.3f;    //The max time allowed between double clicks
        [SerializeField] private float m_SwipeWidth = 0.3f;         //The width of a swipe

        
        private Vector2 m_MouseDownPosition;                        // The screen position of the mouse when Fire1 is pressed.
        private Vector2 m_MouseUpPosition;                          // The screen position of the mouse when Fire1 is released.
        private float m_LastMouseUpTime;                            // The time when Fire1 was last released.
        private float m_LastHorizontalValue;                        // The previous value of the horizontal axis used to detect keyboard swipes.
        private float m_LastVerticalValue;                          // The previous value of the vertical axis used to detect keyboard swipes.
        private float holdDuration;   //Added by osharaki. How long player has been PRESSING touchpad.
        private bool coroutineRunning; //Added by osharaki.
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject targetTree;     
        [SerializeField] private Canvas WegweiserTemplateCanvas;
        [SerializeField] private float distInfrontOfPLayer;
        //[SerializeField] private GameObject dummyRotator;

        public float DoubleClickTime{ get { return m_DoubleClickTime; } }

        /*void Awake()
        {
            //OnClick += UpdateTextOnClick;
            OnClick += OnDemandPlay.instance.ReplayOnClick;
        }*/

        //Added by osharaki
        void Start()
        {
            GameObject WW = GameObject.FindWithTag("Wegweiser");
            if(WW != null)
                WegweiserTemplateCanvas = WW.GetComponent<Canvas>();
            player = GameObject.FindWithTag("Player");
            coroutineRunning = false;
            holdDuration = 0;
            OnDoubleClick += ShowWayStarter;
        }

        private void Update()
        {            
            if (!(Application.platform == RuntimePlatform.WindowsEditor))
            {
                if (!coroutineRunning)
                    StartCoroutine("CheckInputOVR");    //To be used if platform other than editor. Android implied.
            }                
            else
                CheckInput();   //To be used if platform is editor
        }
        
        //Added by osharaki. 
        /// <summary>
        /// Checks for input using Oculus' OVR library.
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckInputOVR()
        {
            coroutineRunning = true;
            while (true)
            {
                if (OVRInput.GetDown(OVRInput.Button.Back))
                {
                    //Debug.Log("look at me! We're going back :) ");
                    
                    //Unsubscribing methods called on fadeout
                    //GameController.instance.myCamera.GetComponent<VRCameraFade>().OnFadeOutComplete -= GameController.instance.LoadNextScene;
                    GameController.instance.myCamera.GetComponent<VRCameraFade>().OnFadeOutComplete += GameController.instance.LoadMenu;
                    Camera.main.GetComponent<VRCameraFade>().FadeOut(false);
                }
                if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
                {
                    // If anything has subscribed to OnUp call it.
                    if (OnUp != null)
                        OnUp();

                    // If the time between the last release of Fire1 and now is less
                    // than the allowed double click time then it's a double click.
                    if (Time.time - m_LastMouseUpTime < m_DoubleClickTime)
                    {
                        // If anything has subscribed to OnDoubleClick call it.
                        if (OnDoubleClick != null)
                            OnDoubleClick();
                    }
                    else
                    {
                        // If it's not a double click, it's a single click.
                        // If anything has subscribed to OnClick call it.
                        if (OnClick != null)
                            OnClick();
                    }

                    // Record the time when Fire1 is released.
                    m_LastMouseUpTime = Time.time;
                }             
                //If touchpad pressed, check how long it's been pressed and
                //trigger event after certain time.   
                if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
                {
                    while (!OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad))
                    {
                        holdDuration += Time.deltaTime;
                        if (holdDuration >= 2) //If touchpad was held down n secs or longer trigger event..
                        {
                            if (OnTouchpadHold != null)
                                OnTouchpadHold();
                            holdDuration = 0;
                        }
                        yield return null;
                    }                    
                }
                yield return null;
            }
        }

        private void CheckInput()
        {
            // Set the default swipe to be none.
            SwipeDirection swipe = SwipeDirection.NONE;

            if (Input.GetButtonDown("Fire1"))
            {
                //Debug.Log("Fire1 pressed!"); //added by osharaki
                // When Fire1 is pressed record the position of the mouse.
                m_MouseDownPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            
                // If anything has subscribed to OnDown call it.
                if (OnDown != null)
                {
                    OnDown();
                    //Debug.Log("Something is subscribed to OnDown"); //added me osharaki
                }                
            }

            // This if statement is to gather information about the mouse when the button is up.
            if (Input.GetButtonUp ("Fire1"))
            {
                // When Fire1 is released record the position of the mouse.
                m_MouseUpPosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

                // Detect the direction between the mouse positions when Fire1 is pressed and released.
                swipe = DetectSwipe ();
            }

            // If there was no swipe this frame from the mouse, check for a keyboard swipe.
            if (swipe == SwipeDirection.NONE)
                swipe = DetectKeyboardEmulatedSwipe();

            // If there are any subscribers to OnSwipe call it passing in the detected swipe.
            if (OnSwipe != null)
                OnSwipe(swipe);

            // This if statement is to trigger events based on the information gathered before.
            if(Input.GetButtonUp ("Fire1"))
            {
                // If anything has subscribed to OnUp call it.
                if (OnUp != null)
                    OnUp();

                // If the time between the last release of Fire1 and now is less
                // than the allowed double click time then it's a double click.
                if (Time.time - m_LastMouseUpTime < m_DoubleClickTime)
                {
                    // If anything has subscribed to OnDoubleClick call it.
                    if (OnDoubleClick != null)
                        OnDoubleClick();
                }
                else
                {
                    // If it's not a double click, it's a single click.
                    // If anything has subscribed to OnClick call it.
                    if (OnClick != null)
                        OnClick();
                }

                // Record the time when Fire1 is released.
                m_LastMouseUpTime = Time.time;
            }

            // If the Cancel button is pressed and there are subscribers to OnCancel call it.
            if (Input.GetButtonDown("Cancel"))
            {
                if (OnCancel != null)
                    OnCancel();
            }
        }


        private SwipeDirection DetectSwipe ()
        {
            // Get the direction from the mouse position when Fire1 is pressed to when it is released.
            Vector2 swipeData = (m_MouseUpPosition - m_MouseDownPosition).normalized;

            // If the direction of the swipe has a small width it is vertical.
            bool swipeIsVertical = Mathf.Abs (swipeData.x) < m_SwipeWidth;

            // If the direction of the swipe has a small height it is horizontal.
            bool swipeIsHorizontal = Mathf.Abs(swipeData.y) < m_SwipeWidth;

            // If the swipe has a positive y component and is vertical the swipe is up.
            if (swipeData.y > 0f && swipeIsVertical)
                return SwipeDirection.UP;

            // If the swipe has a negative y component and is vertical the swipe is down.
            if (swipeData.y < 0f && swipeIsVertical)
                return SwipeDirection.DOWN;

            // If the swipe has a positive x component and is horizontal the swipe is right.
            if (swipeData.x > 0f && swipeIsHorizontal)
                return SwipeDirection.RIGHT;

            // If the swipe has a negative x component and is vertical the swipe is left.
            if (swipeData.x < 0f && swipeIsHorizontal)
                return SwipeDirection.LEFT;

            // If the swipe meets none of these requirements there is no swipe.
            return SwipeDirection.NONE;
        }


        private SwipeDirection DetectKeyboardEmulatedSwipe ()
        {
            // Store the values for Horizontal and Vertical axes.
            float horizontal = Input.GetAxis ("Horizontal");
            float vertical = Input.GetAxis ("Vertical");

            // Store whether there was horizontal or vertical input before.
            bool noHorizontalInputPreviously = Mathf.Abs (m_LastHorizontalValue) < float.Epsilon;
            bool noVerticalInputPreviously = Mathf.Abs(m_LastVerticalValue) < float.Epsilon;

            // The last horizontal values are now the current ones.
            m_LastHorizontalValue = horizontal;
            m_LastVerticalValue = vertical;

            // If there is positive vertical input now and previously there wasn't the swipe is up.
            if (vertical > 0f && noVerticalInputPreviously)
                return SwipeDirection.UP;

            // If there is negative vertical input now and previously there wasn't the swipe is down.
            if (vertical < 0f && noVerticalInputPreviously)
                return SwipeDirection.DOWN;

            // If there is positive horizontal input now and previously there wasn't the swipe is right.
            if (horizontal > 0f && noHorizontalInputPreviously)
                return SwipeDirection.RIGHT;

            // If there is negative horizontal input now and previously there wasn't the swipe is left.
            if (horizontal < 0f && noHorizontalInputPreviously)
                return SwipeDirection.LEFT;

            // If the swipe meets none of these requirements there is no swipe.
            return SwipeDirection.NONE;
        }
        

        private void OnDestroy()
        {
            // Ensure that all events are unsubscribed when this is destroyed.
            OnSwipe = null;
            OnClick = null;
            OnDoubleClick = null;
            OnDown = null;
            OnUp = null;
        }
        
        void UpdateTextOnClick() //added by osharaki
        {
            if (alreadyClicked)
            {
                myTextbox.text = "Waiting...";
                alreadyClicked = !alreadyClicked;
            }
            else
            {
                myTextbox.text = "Clicked!";
                alreadyClicked = !alreadyClicked;
            }
            
        }

        /// <summary>
        /// Called to start coroutine ShowWay.
        /// </summary>
        private void ShowWayStarter()
        {
            if(GameController.instance.sceneName == "Intro")
            {
                GameObject circlingBird = GameObject.FindWithTag("Circling Bird");
                if(circlingBird != null)
                    targetTree = circlingBird.GetComponent<MovingBird>().landingTree;
            }
            else if(GameController.instance.sceneName == "Basic Forest Summer")
            {
                targetTree = AudioController.instance.targetObject;
            }
            else if(GameController.instance.sceneName == "Challenge Forest Summer")
            {
                targetTree = ChallengeAudioController.instance.targetObject;
            }
            if (targetTree != null)
                StartCoroutine(ShowWay(targetTree));
            else
                Debug.Log("No target tree");
            //ShowWay2(targetTree, leftFootRightFoot);
        }        

        private IEnumerator ShowWayOld(GameObject targetTree, Image[] footStepsIcons) //
        {
            float initialYRot = player.transform.rotation.eulerAngles.y;
            float finalYRot;
            Debug.Log("initial y-Rot: " + initialYRot);
            WegweiserTemplateCanvas.transform.parent.position =
                new Vector3(player.transform.position.x,
                            WegweiserTemplateCanvas.transform.parent.position.y,
                            player.transform.position.z);
            WegweiserTemplateCanvas.transform.parent.rotation =
                    Quaternion.Euler(
                    WegweiserTemplateCanvas.transform.parent.rotation.x,
                    player.transform.rotation.eulerAngles.y,
                    WegweiserTemplateCanvas.transform.parent.rotation.z);
            //rotate player to face tree
            player.transform.rotation = Quaternion.LookRotation(player.transform.position -
                targetTree.transform.position); //rotates the billboard
                                                //locks rotation on x-z-axes
            player.transform.rotation = Quaternion.Euler(0,
                                                  player.transform.rotation.eulerAngles.y,
                                                  0);
            finalYRot = 180 - player.transform.rotation.eulerAngles.y;
            //finalYRot = player.transform.rotation.eulerAngles.y * (-1);
            //finalYRot = Quaternion.Inverse(player.transform.rotation).eulerAngles.y;
            //if(Math.Abs(initialYRot - finalYRot) > 180)
            Debug.Log("Final y-Rot: " + finalYRot);

            Vector2 targetTree2DPos = new Vector2(targetTree.transform.position.x,
                                        targetTree.transform.position.z);
            Vector2 player2DPos = new Vector2(player.transform.position.x,
                                            player.transform.position.z);
            float distToTree = Vector2.Distance(player2DPos,
                                                targetTree2DPos);
            int numOfSteps = (int)(distToTree * 4 / 15); //4 steps per 15 meters
            GameObject[] stepsArray = null;
            //Number of steps cannot exceed 6
            if (numOfSteps <= WegweiserTemplateCanvas.transform.childCount)
            {
                stepsArray = new GameObject[numOfSteps];
            }
            else
            {
                stepsArray = new GameObject[WegweiserTemplateCanvas.transform.childCount];
            }
            
            foreach (Transform t in WegweiserTemplateCanvas.transform)
            {
                t.gameObject.SetActive(false);
            }
            for (int i = 0; i < stepsArray.Length; i++)
            {
                if (i <= WegweiserTemplateCanvas.transform.childCount) //If statement may be redundant
                {
                    stepsArray[i] = WegweiserTemplateCanvas.transform.GetChild(i).gameObject;
                    //stepsArray[i].SetActive(false); //Provides a clean slate for each new show way request.

                    float nextYRot = Mathf.Lerp(initialYRot,
                                                finalYRot,
                                                ((float)i / stepsArray.Length));

                    /*stepsArray[i].transform.rotation =
                        Quaternion.Euler(
                        stepsArray[i].transform.rotation.eulerAngles.x,
                        stepsArray[i].transform.rotation.eulerAngles.y,
                        nextYRot * -1);*/
                    stepsArray[i].transform.rotation =
                        Quaternion.Euler(
                        stepsArray[i].transform.rotation.eulerAngles.x,
                        nextYRot,
                        stepsArray[i].transform.rotation.eulerAngles.z);
                    if (i == 0)
                    {
                        stepsArray[i].transform.position =
                        new Vector3(player.transform.position.x,
                                    stepsArray[i].transform.position.y,
                                    player.transform.position.z);
                        stepsArray[i].transform.position +=
                            player.transform.forward * (i - 2);                        
                    }
                    else
                    {
                        //place feet apart
                        if(i % 2 == 0) //if even, i.e. right foot
                        {
                            stepsArray[i].transform.position = 
                                stepsArray[i - 1].transform.position;
                            stepsArray[i].transform.position +=
                                stepsArray[i - 1].transform.right; 
                            /*stepsArray[i].transform.position =
                            new Vector3(stepsArray[i - 1].transform.position.x + 1f,
                            stepsArray[i - 1].transform.position.y,
                            stepsArray[i - 1].transform.position.z);*/
                        }
                        else //if odd, i.e. left foot
                        {
                            stepsArray[i].transform.position =
                                stepsArray[i - 1].transform.position;
                            stepsArray[i].transform.position +=
                                stepsArray[i - 1].transform.right * (-1);
                            /*stepsArray[i].transform.position =
                            new Vector3(stepsArray[i - 1].transform.position.x - 1f,
                            stepsArray[i - 1].transform.position.y,
                            stepsArray[i - 1].transform.position.z);*/
                        }
                        
                        stepsArray[i].transform.position +=
                            stepsArray[i - 1].transform.up * (2);
                    }
                    
                    /*stepsArray[i].transform.position =
                        new Vector3(player.transform.position.x,
                                    stepsArray[i].transform.position.y,
                                    player.transform.position.z + (i + 6));*/
                    /*stepsArray[i].transform.position +=
                        player.transform.forward * (1 + i + 2);*/
                    stepsArray[i].SetActive(true);
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }


        /// <summary>
        /// Displays a series of footprints that lead the player
        /// towards a target location.
        /// </summary>
        /// <param name="targetTree">Tree that footprints are 
        /// leading to.</param>
        /// <returns></returns>
        private IEnumerator ShowWay(GameObject targetTree) //
        {
            float initialYRot = player.transform.rotation.eulerAngles.y;
            float finalYRot;
            //Debug.Log("initial y-Rot: " + initialYRot);
            WegweiserTemplateCanvas.transform.parent.position =
                new Vector3(player.transform.position.x,
                            WegweiserTemplateCanvas.transform.parent.position.y,
                            player.transform.position.z);
            WegweiserTemplateCanvas.transform.parent.rotation =
                    Quaternion.Euler(
                    WegweiserTemplateCanvas.transform.parent.rotation.x,
                    player.transform.rotation.eulerAngles.y,
                    WegweiserTemplateCanvas.transform.parent.rotation.z);
            //rotate player to face tree
            player.transform.rotation = Quaternion.LookRotation(player.transform.position -
                targetTree.transform.position); //rotates the billboard
                                                //locks rotation on x-z-axes
            player.transform.rotation = Quaternion.Euler(0,
                                                  player.transform.rotation.eulerAngles.y,
                                                  0);
            finalYRot = player.transform.rotation.eulerAngles.y - 180;
            //finalYRot = player.transform.rotation.eulerAngles.y;
            //finalYRot = 180 - player.transform.rotation.eulerAngles.y;
            //finalYRot = player.transform.rotation.eulerAngles.y * (-1);
            //finalYRot = Quaternion.Inverse(player.transform.rotation).eulerAngles.y;
            /*dummyRotator.transform.position = new Vector3(
                player.transform.position.x,
                dummyRotator.transform.position.y,
                player.transform.position.z);
            dummyRotator.transform.rotation = Quaternion.Euler(
                dummyRotator.transform.rotation.eulerAngles.x,
                finalYRot,
                dummyRotator.transform.rotation.eulerAngles.z);*/  
            
            //Debug.Log("Final y-Rot: " + finalYRot);
            Vector2 targetTree2DPos = new Vector2(targetTree.transform.position.x,
                                        targetTree.transform.position.z);
            Vector2 player2DPos = new Vector2(player.transform.position.x,
                                            player.transform.position.z);
            float distToTree = Vector2.Distance(player2DPos,
                                                targetTree2DPos);
            int numOfSteps = (int)(distToTree * 4 / 15); //4 steps per 15 meters
            GameObject[] stepsArray = null;
            //Number of steps cannot exceed 6
            if (numOfSteps <= WegweiserTemplateCanvas.transform.childCount)
            {
                stepsArray = new GameObject[numOfSteps];
            }
            else
            {
                stepsArray = new GameObject[WegweiserTemplateCanvas.transform.childCount];
            }

            foreach (Transform t in WegweiserTemplateCanvas.transform)
            {
                t.gameObject.SetActive(false);
            }

            //Rotate right or left??
            if (Math.Abs(initialYRot - finalYRot) < 180) //rotate normally
            {
                
                for (int i = 0; i < stepsArray.Length; i++)
                {
                    if (i <= WegweiserTemplateCanvas.transform.childCount) //If statement may be redundant
                    {
                        stepsArray[i] = WegweiserTemplateCanvas.transform.GetChild(i).gameObject;
                        //stepsArray[i].SetActive(false); //Provides a clean slate for each new show way request.
                        /*
                         * 1) Interpolate from initial to final angle over
                         * given number of steps
                         * 2)Assign angle to appropriate step and move forward
                         * a certain distance                     
                         */
                        float nextYRot = Mathf.Lerp(initialYRot,
                                                    finalYRot,
                                                    ((float)i / stepsArray.Length));

                        /*stepsArray[i].transform.rotation =
                            Quaternion.Euler(
                            stepsArray[i].transform.rotation.eulerAngles.x,
                            stepsArray[i].transform.rotation.eulerAngles.y,
                            nextYRot * -1);*/
                        stepsArray[i].transform.rotation =
                            Quaternion.Euler(
                            stepsArray[i].transform.rotation.eulerAngles.x,
                            nextYRot,
                            stepsArray[i].transform.rotation.eulerAngles.z);
                        if (i == 0)
                        {
                            stepsArray[i].transform.position =
                            new Vector3(player.transform.position.x,
                                        stepsArray[i].transform.position.y,
                                        player.transform.position.z);
                            stepsArray[i].transform.position +=
                                stepsArray[i].transform.up * (i + distInfrontOfPLayer);
                        }
                        else
                        {
                            //place feet apart
                            if (i % 2 == 0) //if even, i.e. right foot
                            {
                                stepsArray[i].transform.position =
                                    stepsArray[i - 1].transform.position;
                                stepsArray[i].transform.position +=
                                    stepsArray[i - 1].transform.right;
                                /*stepsArray[i].transform.position =
                                new Vector3(stepsArray[i - 1].transform.position.x + 1f,
                                stepsArray[i - 1].transform.position.y,
                                stepsArray[i - 1].transform.position.z);*/
                            }
                            else //if odd, i.e. left foot
                            {
                                stepsArray[i].transform.position =
                                    stepsArray[i - 1].transform.position;
                                stepsArray[i].transform.position +=
                                    stepsArray[i - 1].transform.right * (-1);
                                /*stepsArray[i].transform.position =
                                new Vector3(stepsArray[i - 1].transform.position.x - 1f,
                                stepsArray[i - 1].transform.position.y,
                                stepsArray[i - 1].transform.position.z);*/
                            }

                            stepsArray[i].transform.position +=
                                stepsArray[i - 1].transform.up * (2);
                        }

                        /*stepsArray[i].transform.position =
                            new Vector3(player.transform.position.x,
                                        stepsArray[i].transform.position.y,
                                        player.transform.position.z + (i + 6));*/
                        /*stepsArray[i].transform.position +=
                            player.transform.forward * (1 + i + 2);*/
                        stepsArray[i].SetActive(true);
                        yield return new WaitForSeconds(0.5f);
                    }
                }                        
            }
            else //Need to rotate the other way
            {
                float angleDiff = Mathf.DeltaAngle(initialYRot, finalYRot);
                if(initialYRot > finalYRot)
                {
                    finalYRot = initialYRot + angleDiff;
                }
                else if(initialYRot < finalYRot)
                {
                    finalYRot = initialYRot - angleDiff;
                }
                //finalYRot = initialYRot - angleDiff;

                for (int i = 0; i < stepsArray.Length; i++)
                {
                    if (i <= WegweiserTemplateCanvas.transform.childCount) //If statement may be redundant
                    {
                        stepsArray[i] = WegweiserTemplateCanvas.transform.GetChild(i).gameObject;
                        //stepsArray[i].SetActive(false); //Provides a clean slate for each new show way request.
                        /*
                         * 1) Interpolate from initial to final angle over
                         * given number of steps
                         * 2)Assign angle to appropriate step and move forward
                         * a certain distance                     
                         */
                        float nextYRot = Mathf.Lerp(initialYRot,
                                                    finalYRot,
                                                    ((float)i / stepsArray.Length));

                        /*stepsArray[i].transform.rotation =
                            Quaternion.Euler(
                            stepsArray[i].transform.rotation.eulerAngles.x,
                            stepsArray[i].transform.rotation.eulerAngles.y,
                            nextYRot * -1);*/
                        stepsArray[i].transform.rotation =
                            Quaternion.Euler(
                            stepsArray[i].transform.rotation.eulerAngles.x,
                            nextYRot,
                            stepsArray[i].transform.rotation.eulerAngles.z);
                        if (i == 0)
                        {
                            stepsArray[i].transform.position =
                            new Vector3(player.transform.position.x,
                                        stepsArray[i].transform.position.y,
                                        player.transform.position.z);
                            stepsArray[i].transform.position +=
                                stepsArray[i].transform.up * (i + distInfrontOfPLayer);
                        }
                        else
                        {
                            //place feet apart
                            if (i % 2 == 0) //if even, i.e. right foot
                            {
                                stepsArray[i].transform.position =
                                    stepsArray[i - 1].transform.position;
                                stepsArray[i].transform.position +=
                                    stepsArray[i - 1].transform.right;
                                /*stepsArray[i].transform.position =
                                new Vector3(stepsArray[i - 1].transform.position.x + 1f,
                                stepsArray[i - 1].transform.position.y,
                                stepsArray[i - 1].transform.position.z);*/
                            }
                            else //if odd, i.e. left foot
                            {
                                stepsArray[i].transform.position =
                                    stepsArray[i - 1].transform.position;
                                stepsArray[i].transform.position +=
                                    stepsArray[i - 1].transform.right * (-1);
                                /*stepsArray[i].transform.position =
                                new Vector3(stepsArray[i - 1].transform.position.x - 1f,
                                stepsArray[i - 1].transform.position.y,
                                stepsArray[i - 1].transform.position.z);*/
                            }

                            stepsArray[i].transform.position +=
                                stepsArray[i - 1].transform.up * (2);
                        }

                        /*stepsArray[i].transform.position =
                            new Vector3(player.transform.position.x,
                                        stepsArray[i].transform.position.y,
                                        player.transform.position.z + (i + 6));*/
                        /*stepsArray[i].transform.position +=
                            player.transform.forward * (1 + i + 2);*/
                        stepsArray[i].SetActive(true);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            foreach(GameObject step in stepsArray)
            {
                step.SetActive(false);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}