using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTriggerHandler : MonoBehaviour
{
    public Animator playerAnimator;
    public GameObject player;
    public GameObject cloudFactoryUI;
    public GameObject mainGameUI;
    public Joystick joystickController;
    public float cooldownTime = 1.0f;
    public ContentManager contentManager;

    [SerializeField] private Slider actionSliderPrefab;
    private Slider actionSliderInstance;
    private float actionFillDuration = 1f;
    private Coroutine actionCoroutine;

    private PlayerController playerController;
    private bool isCooldownActive = false;

    public List<GameObject> showOnActiveBrewery = new List<GameObject>();
    public List<GameObject> hideOnActive = new List<GameObject>();

    public GameObject brewCamera;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Portal"))
        {
            playerAnimator.SetBool("isTeleporting", true);
        }
        else if (collider.CompareTag("Respawn"))
        {
            player.transform.position = new Vector3(0, 8, 0);
        }
        else if ((collider.CompareTag("Cloud Factory") || collider.CompareTag("Brew")) && !isCooldownActive)
        {
            if (actionCoroutine == null)
            {
                actionCoroutine = StartCoroutine(StartActionFill(collider));
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Portal"))
        {
            playerAnimator.SetBool("isTeleporting", false);
        }
        else if (collider.CompareTag("Cloud Factory") || collider.CompareTag("Brew"))
        {
            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
                actionCoroutine = null;
            }

            if (actionSliderInstance != null)
            {
                Destroy(actionSliderInstance.gameObject);
            }
        }
    }

    private IEnumerator StartActionFill(Collider collider)
    {
        Vector3 offsetPosition = collider.transform.position + Vector3.up * 100f;
        actionSliderInstance = Instantiate(actionSliderPrefab, offsetPosition, Quaternion.identity);
        actionSliderInstance.transform.SetParent(FindObjectOfType<Canvas>().transform, false);

        float fillTime = 0f;
        while (fillTime < actionFillDuration)
        {
            fillTime += Time.deltaTime;
            actionSliderInstance.value = fillTime / actionFillDuration;
            yield return null;
        }

        ActivateActionObject(collider);

        Destroy(actionSliderInstance.gameObject);
        actionCoroutine = null;
    }

    private void ActivateActionObject(Collider collider)
    {
        if (collider.CompareTag("Cloud Factory"))
        {
            ActivateCloudTrigger();
        }
        else if (collider.CompareTag("Brew"))
        {
            ActivateBrewTrigger();
        }
    }

    private void ActivateCloudTrigger()
    {
        playerController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        cloudFactoryUI.SetActive(true);
        foreach (GameObject go in hideOnActive) go.SetActive(false);
        joystickController.ResetJoystick();
    }

    private void ActivateBrewTrigger()
    {
        playerController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        brewCamera.SetActive(true);
        foreach (GameObject go in hideOnActive) go.SetActive(false);
        joystickController.ResetJoystick();
        contentManager.UpdateInventoryUI();
        StartCoroutine(WaitAndEnableBrewUI());
    }

    public void HandleActionExit()
    {
        foreach (GameObject go in hideOnActive)
        {
            go.SetActive(true);
        }
    }

    private IEnumerator WaitAndEnableBrewUI() {
        yield return new WaitForSeconds(2f);
        foreach (GameObject go in showOnActiveBrewery) go.SetActive(true);
    }

    public void HandleBrewExit() {
        foreach (GameObject go in showOnActiveBrewery)
        {
            go.SetActive(false);
        }

        brewCamera.SetActive(false);

        StartCoroutine(WaitAndDisableBrewUI());
    }

    public IEnumerator WaitAndDisableBrewUI() {
        yield return new WaitForSeconds(2f);
        foreach (GameObject go in hideOnActive) go.SetActive(true);
    }
}
