using System.Collections;
using UnityEngine;

public class Amino_acid_ScrollController : MonoBehaviour
{
    //아미노산 스크롤 안에 각 그룹 펼치거나 접는 기능
    //나중에 한곳에 모든 스크롤 그룹 통합예정
    public GameObject Nonpolar_Panel;
    public GameObject Polar_Panel;
    public GameObject Acidic_Panel;
    public GameObject Basic_Panel;

    public void NonpolarGroup()
    {

        Nonpolar_Panel.SetActive(!Nonpolar_Panel.activeSelf);
    }
    public void PolarGroup()
    {

        Polar_Panel.SetActive(!Polar_Panel.activeSelf);
    }
    public void AcidicGroup()
    {

        Acidic_Panel.SetActive(!Acidic_Panel.activeSelf);
    }
    public void BasicGroup()
    {
        Basic_Panel.SetActive(!Basic_Panel.activeSelf);
    }

}
