using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [SerializeField] private float destroyTimer = 2f;
    [SerializeField] private float shotPower = 500f;
    [SerializeField] private float ejectPower = 150f;

    private InputDevice rightHand;
    private XRGrabInteractable grabInteractable;

    private bool isHeld = false;
    private bool lastTriggerState = false;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        grabInteractable = GetComponent<XRGrabInteractable>();

        // Track grab state properly
        grabInteractable.selectEntered.AddListener(_ => isHeld = true);
        grabInteractable.selectExited.AddListener(_ => isHeld = false);
    }

    void Update()
    {
        if (!rightHand.isValid)
            rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed))
        {
            // Fire only once per click AND only if held
            if (isPressed && !lastTriggerState && isHeld)
            {
                gunAnimator.SetTrigger("Fire");
            }

            lastTriggerState = isPressed;
        }
    }

    // Called by animation event
    void Shoot()
    {
        if (muzzleFlashPrefab)
        {
            GameObject tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
            Destroy(tempFlash, destroyTimer);
        }

        if (!bulletPrefab) return;

        Rigidbody rb = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation)
            .GetComponent<Rigidbody>();

        rb.AddForce(barrelLocation.forward * shotPower);
    }

    // Called by animation event
    void CasingRelease()
    {
        if (!casingExitLocation || !casingPrefab) return;

        GameObject tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation);

        Rigidbody rb = tempCasing.GetComponent<Rigidbody>();

        rb.AddExplosionForce(
            Random.Range(ejectPower * 0.7f, ejectPower),
            casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f,
            1f
        );

        rb.AddTorque(
            new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)),
            ForceMode.Impulse
        );

        Destroy(tempCasing, destroyTimer);
    }
}