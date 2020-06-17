﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] Animator handAnimator;
    [SerializeField] Transform firePoint;
    [SerializeField] Camera playerCamera;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject wallHitDecalPrefab;
    [SerializeField] GameObject projectilePrefab;

    public Gun equippedWeapon;
    private PlayerController playerController;

    private float lastTimeShot = 0f;
    private int currentRecoilIndex = 0;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        equippedWeapon = new AK47();
    }

    void Update()
    {
        bool isTryingToShoot = Input.GetKey(KeyCode.Mouse0);

        if (isTryingToShoot)
        {
            HandleShooting();
        } else
        {
            // Start trying to reset back to position we are not shooting anymore
            playerController.SetGunRotation(
                Vector3.Lerp(
                    playerController.gunRotation,
                    Vector3.zero,
                    equippedWeapon.fireRatePerSecond * Time.deltaTime
                )
            );
        }
    }

    void HandleShooting()
    {
        if (Time.time - lastTimeShot >= 1 / equippedWeapon.fireRatePerSecond)
        {
            PlayAttackAnimation();

            muzzleFlash.Play();

            HandleGunRecoil();

            Instantiate(projectilePrefab, firePoint.transform.position, firePoint.rotation);

            RaycastHit hit;
            if (Physics.Raycast(
                firePoint.transform.position,
                firePoint.transform.TransformDirection(Vector3.forward),
                out hit,
                equippedWeapon.fallOffDistance))
            {
                Instantiate(wallHitDecalPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }

            lastTimeShot = Time.time;
        }
    }

    void HandleGunRecoil()
    {
        if (Time.time - lastTimeShot >= equippedWeapon.recoilResetTimeSeconds)
        {
            // Reset to 0, player hasnt shot in enough time so we reset recoil pattern
            playerController.SetGunRotation(playerController.gunRotation + equippedWeapon.recoilPattern[0]);
            currentRecoilIndex = 1;
        } else
        {
            playerController.SetGunRotation(playerController.gunRotation + equippedWeapon.recoilPattern[currentRecoilIndex]);

            if (currentRecoilIndex + 1 <= equippedWeapon.recoilPattern.Length - 1)
            {
                currentRecoilIndex += 1;
            } else
            {
                currentRecoilIndex = 0;
            }
        }
    }

    void PlayAttackAnimation()
    {
        handAnimator.Play("Fire");
    }
}