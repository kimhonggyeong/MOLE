using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Detail_Infromation_Btn : MonoBehaviour
{
    public MainBtnController MBC; // 회전 토글 버튼 스크립트 참조
    public TextMeshProUGUI InformationName;

    public GameObject Basic_property_Information;
    public GameObject Physical_property_Information;
    public GameObject Chemical_property_Information;
    public GameObject Exceptions_Information;
    public GameObject RelevantLink_Information;

    public TextMeshProUGUI Basic_property_Name;
    public TextMeshProUGUI Basic_property_IUPAC;
    public TextMeshProUGUI Basic_property_Acronym;
    public TextMeshProUGUI Basic_property_Classification;
    public TextMeshProUGUI Basic_property_ChemicalFormula;
    public TextMeshProUGUI Basic_property_MolecularWeight;

    public TextMeshProUGUI Physical_property_State;
    public TextMeshProUGUI Physical_property_Color;
    public TextMeshProUGUI Physical_property_MeltingPoint;
    public TextMeshProUGUI Physical_property_Solubility;
    public TextMeshProUGUI Physical_property_pKa;

    public TextMeshProUGUI Chemical_property_a_ArminoAcid;
    public TextMeshProUGUI Chemical_property_SideChain;
    public TextMeshProUGUI Chemical_property_Stereochemistry;
    public TextMeshProUGUI Chemical_property_Symmetry;

    public TextMeshProUGUI Exceptions_StereoisomersExist;
    public TextMeshProUGUI Exceptions_Scanning;

    public TextMeshProUGUI RelevantLink1;
    public TextMeshProUGUI RelevantLink2;
    public TextMeshProUGUI RelevantLink3;

    public void Basic_property_Open() 
    {
        InformationName.text = "기본성질";
        CloseInformation();
        Basic_property_Information.SetActive(true);
        SetDetailText();
    }
    public void Physical_property_Open()
    {
        InformationName.text = "물리적 성질";
        CloseInformation();
        Physical_property_Information.SetActive(true);
    }
    public void Chemical_property_Open()
    {
        InformationName.text = "화학적 특징";
        CloseInformation();
        Chemical_property_Information.SetActive(true);
    }
    public void Exceptions_Open()
    {
        InformationName.text = "특이사항";
        CloseInformation();
        Exceptions_Information.SetActive(true);
    }
    public void RelevantLink_Open()
    {
        InformationName.text = "관련링크";
        CloseInformation();
        RelevantLink_Information.SetActive(true);
    }
    void CloseInformation()
    {
        Basic_property_Information.SetActive(false);
        Physical_property_Information.SetActive(false);
        Chemical_property_Information.SetActive(false);
        Exceptions_Information.SetActive(false);
        RelevantLink_Information.SetActive(false);
    }
    void SetDetailText()
    {
        if (MBC.NowModelName == "Alanine")
        {
            Basic_property_Name.text = "알라닌 (Alanine)";
            Basic_property_IUPAC.text = "2-아미노프로판산 (2-Aminopropanoic acid)";
            Basic_property_Acronym.text = "Ala 또는 A";
            Basic_property_Classification.text = "비극성, 비필수 α-아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>3</sub>H<sub>7</sub>NO<sub>2</sub>";
            Basic_property_MolecularWeight.text = "89.09 g/mol";

            Physical_property_State.text = "고체 (결정성)";
            Physical_property_Color.text = "백색 또는 무색";
            Physical_property_MeltingPoint.text = "297°C (분해 동반)";
            Physical_property_Solubility.text = "물에 잘 녹음, 에탄올엔 거의 불용";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.35\r\npKa2 (NH₃⁺): 약 9.87\r\n등전점(pI): 약 6.01";

            Chemical_property_a_ArminoAcid.text = "중심에 α-탄소, NH₂, COOH, H, 메틸기(-CH₃) 존재";
            Chemical_property_SideChain.text = "단순한 메틸기 (-CH₃) → 가장 작고 비극성적인 곁사슬 중 하나";
            Chemical_property_Stereochemistry.text = "천연 알라닌은 L-알라닌, D형은 드물게 발견";
            Chemical_property_Symmetry.text = "카이랄 중심을 가짐 (4개의 서로 다른 치환기)";

            Exceptions_StereoisomersExist.text = "L-알라닌이 생체 내 주형, D-알라닌은 세균 세포벽 등 특수 환경에서 존재";
            Exceptions_Scanning.text = "단백질 서열에서 중요한 잔기를 알라닌으로 치환하여 기능 평가하는 실험 (Alanine Scanning)";

           /* //RelevantLink1.text = "https://ko.wikipedia.org/wiki/알라닌";
            //RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Alanine";
            //RelevantLink3.text = "https://scholar.google.com/scholar?q=alanine+biological+function";*/

        }
        else if (MBC.NowModelName == "Valine")
        {
            Basic_property_Name.text = "발린 (Valine)";
            Basic_property_IUPAC.text = "2-아미노-3-메틸부탄산 (2-Amino-3-methylbutanoic acid)";
            Basic_property_Acronym.text = "Val 또는 V";
            Basic_property_Classification.text = "필수 아미노산 / 비극성 / 가지사슬형 아미노산 (BCAA)";
            Basic_property_ChemicalFormula.text = "C<sub>5</sub>H<sub>11</sub>NO<sub>2</sub>";
            Basic_property_MolecularWeight.text = "117.15 g/mol";

            Physical_property_State.text = "고체 (결정성)";
            Physical_property_Color.text = "백색 또는 무색 결정";
            Physical_property_MeltingPoint.text = "약 315 °C (분해 동반)";
            Physical_property_Solubility.text = "물에는 잘 녹음, 알코올에는 제한적";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.32\r\npKa2 (NH₃⁺): 약 9.62\r\n등전점(pI): 약 6.0";

            Chemical_property_a_ArminoAcid.text = "가지사슬형 아미노산(BCAA)의 대표 주자 중 하나 (Leucine, Isoleucine과 함께)";
            Chemical_property_SideChain.text = "곁사슬에 이소프로필기 (-CH(CH₃)₂)를 가짐 → 소수성/비극성";
            Chemical_property_Stereochemistry.text = "입체 중심(Cα)에 대해 L-형만 생체 내 사용";
            Chemical_property_Symmetry.text = "단백질 내에서는 주로 소수성 코어에 위치하여 단백질 안정성에 기여";

            Exceptions_StereoisomersExist.text = "자연계에서는 L-Valine만 존재, D-Valine은 인공적 또는 세균 내 일부 존재 가능";
            Exceptions_Scanning.text = "가지사슬 구조로 인해 단백질 내에서 공간 제약에 민감";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/발린";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Valin";
            RelevantLink3.text = "https://www.frontiersin.org/journals/nutrition/articles/10.3389/fnut.2024.1379390/full?utm_source=chatgpt.com";
*/
        }
        else if (MBC.NowModelName == "Glycine")
        {
            Basic_property_Name.text = "글라이신 (Glycine)";
            Basic_property_IUPAC.text = "2-Aminoethanoic acid";
            Basic_property_Acronym.text = "Gly 또는 G";
            Basic_property_Classification.text = "비극성 아미노산 / 비필수 / 가장 단순한 α-아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>2</sub>H<sub>5</sub>NO<sub>2</sub>";  // Glycine 
            Basic_property_MolecularWeight.text = "75.07 g/mol";

            Physical_property_State.text = "고체 (결정성)";
            Physical_property_Color.text = "백색 결정";
            Physical_property_MeltingPoint.text = "233 °C (분해됨)";
            Physical_property_Solubility.text = "물에 잘 녹음, 극성 용매에 용해 가능";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.34\r\npKa2 (NH₃⁺): 약 9.60\r\n등전점(pI): 약 6.06";

            Chemical_property_a_ArminoAcid.text = "가장 단순한 아미노산: 곁사슬이 수소 원자(H) → 카이랄 중심이 없음";
            Chemical_property_SideChain.text = "비카이랄: 유일하게 광학 이성질체가 없는 아미노산";
            Chemical_property_Stereochemistry.text = "유연성 높음: 구조가 작고 단순하여 단백질의 유연한 부위 (loop, turn)에 자주 위치";
            Chemical_property_Symmetry.text = "광학적 활성이 없음 → 대칭 구조";

            Exceptions_StereoisomersExist.text = "유일하게 입체중심(Cα)이 없는 α-아미노산으로 L/D 구분 없음";
            Exceptions_Scanning.text = "단백질의 특수 위치(loop, hinge 등)에 적합하여 구조 유연성 실험 시 활용됨";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/글라이신";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Glycine";
            RelevantLink3.text = "https://jneuroinflammation.biomedcentral.com/articles/10.1186/s12974-020-01989-w";
*/
        }
        else if (MBC.NowModelName == "Lysine")
        {
            Basic_property_Name.text = "리신 (Lysine)";
            Basic_property_IUPAC.text = "(2S)-2,6-디아미노사노산 ((2S)-2,6-diaminohexanoic acid)";
            Basic_property_Acronym.text = "Lys 또는 K";
            Basic_property_Classification.text = "필수 아미노산, 염기성 아미노산, 단백질 생성 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>6</sub>H<sub>14</sub>N<sub>2</sub>O<sub>2</sub>";  // Lysine  
            Basic_property_MolecularWeight.text = "146.19 g/mol";

            Physical_property_State.text = "고체 (결정성 또는 분말 형태)";
            Physical_property_Color.text = "흰색에서 연한 노란색";
            Physical_property_MeltingPoint.text = "약 215°C (분해점)";
            Physical_property_Solubility.text = "물에 잘 용해됨 (20°C에서 약 0.1 g/mL)";
            Physical_property_pKa.text = "a-카복시기: 약 2.16\r\na-아미노기: 약 9.06\r\ne-아미노기: 약 10.67\r\npH (1% 수용액): 약 9.74";

            Chemical_property_a_ArminoAcid.text = "생체 내에서 L-형태로 존재하며, α-탄소에 단일 입체중심이 있음";
            Chemical_property_SideChain.text = "e-아미노기(NH₂)를 포함한 긴 곁사슬 → 양전하, 수용성과 상호작용에 기여";
            Chemical_property_Stereochemistry.text = "L-리신만 생체에서 사용됨, 광학이성질체 존재";
            Chemical_property_Symmetry.text = "입체중심 1개 → 카이랄 구조";

            Exceptions_StereoisomersExist.text = "단백질 합성에 사용되는 형태는 L-리신이며, e-아미노기의 양전하로 인해 단백질 표면에 위치";
            Exceptions_Scanning.text = "e-아미노기는 다양한 반응성으로 구조 조절 실험 및 아세틸화 연구에 사용됨";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/라이신";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Lysine";
            RelevantLink3.text = "https://www.healthline.com/health/lysine-for-cold-sores";*/
        }
        else if (MBC.NowModelName == "Histidine")
        {
            Basic_property_Name.text = "히스티딘 (Histidine)";
            Basic_property_IUPAC.text = "(2S)-2-아미노-3-(1H-이미다졸-4-일)프로판산";
            Basic_property_Acronym.text = "His 또는 H";
            Basic_property_Classification.text = "필수 아미노산, 극성 아미노산, 방향족 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>6</sub>H<sub>9</sub>N<sub>3</sub>O<sub>2</sub>";  // Histidine 
            Basic_property_MolecularWeight.text = "155.15 g/mol";

            Physical_property_State.text = "고체 (결정성 또는 분말 형태)";
            Physical_property_Color.text = "흰색";
            Physical_property_MeltingPoint.text = "약 287°C (분해점)";
            Physical_property_Solubility.text = "물에 잘 용해됨 (20°C에서 약 45.6 mg/mL)";
            Physical_property_pKa.text = "a-카복시기: 약 1.8\r\na-아미노기: 약 9.3\r\n이미다졸 곁사슬: 약 6.0";

            Chemical_property_a_ArminoAcid.text = "생체 내에서 L-형태로 존재하며, α-탄소에 단일 입체중심이 있음";
            Chemical_property_SideChain.text = "이미다졸 고리 포함 → pH 6.0 부근에서 양전하/중성 사이 변화";
            Chemical_property_Stereochemistry.text = "L-히스티딘만 단백질 생합성에 사용됨";
            Chemical_property_Symmetry.text = "입체중심 하나를 가진 카이랄 구조";

            Exceptions_StereoisomersExist.text = "L-형만 생체에 존재하며, 다양한 대사물질의 전구체 (히스타민 등)";
            Exceptions_Scanning.text = "효소의 활성 부위에서 산-염기 촉매로 작용하며 금속 이온과 결합 가능";

           /* RelevantLink1.text = "https://ko.wikipedia.org/wiki/히스티딘";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Histidine";
            RelevantLink3.text = "https://www.ncbi.nlm.nih.gov/books/NBK537290/";*/
        }
        else if (MBC.NowModelName == "Asparticacid")
        {
            Basic_property_Name.text = "아스파르트산 (Aspartic acid)";
            Basic_property_IUPAC.text = "2-아미노부탄디산 (2-aminobutanedioic acid)";
            Basic_property_Acronym.text = "Asp 또는 D";
            Basic_property_Classification.text = "비필수 아미노산, 산성 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>4</sub>H<sub>7</sub>NO<sub>4</sub>";  // Aspartic acid  
            Basic_property_MolecularWeight.text = "133.10 g/mol";

            Physical_property_State.text = "고체 (결정성 또는 분말 형태)";
            Physical_property_Color.text = "흰색";
            Physical_property_MeltingPoint.text = "약 270°C (분해점)";
            Physical_property_Solubility.text = "물에 약간 용해됨 (25°C에서 약 4.5 g/L)";
            Physical_property_pKa.text = "a-카복시기: 약 1.99\r\nβ-카복시기: 약 3.90\r\na-아미노기: 약 9.90";

            Chemical_property_a_ArminoAcid.text = "생체 내에서 L-형태로 존재하며, α-탄소에 단일 입체중심이 있음";
            Chemical_property_SideChain.text = "β-카복시기를 포함 → 생리적 pH에서 음전하를 띰";
            Chemical_property_Stereochemistry.text = "광학 이성질체 존재하나 생체 내에선 L-Asp만 사용";
            Chemical_property_Symmetry.text = "카이랄 중심 존재";

            Exceptions_StereoisomersExist.text = "D-아스파르트산은 신경계 발달, 호르몬 조절 등 특수 역할 수행";
            Exceptions_Scanning.text = "두 개의 카복시기와 아미노기를 통한 이온화 상태 변화로 기능적 위치 실험 가능";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/아스파르트산";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Aspartic-acid";
            RelevantLink3.text = "https://www.sciencedirect.com/topics/biochemistry-genetics-and-molecular-biology/aspartic-acid";*/
        }
        else if (MBC.NowModelName == "Glutamicacid")
        {
            Basic_property_Name.text = "글루탐산 (Glutamic acid)";
            Basic_property_IUPAC.text = "(2S)-2-아미노펜탄디오산";
            Basic_property_Acronym.text = "Glu 또는 E";
            Basic_property_Classification.text = "비필수 아미노산, 산성 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>5</sub>H<sub>9</sub>NO<sub>4</sub>";  // Glutamic acid  
            Basic_property_MolecularWeight.text = "147.13 g/mol";

            Physical_property_State.text = "고체 (결정성 또는 분말 형태)";
            Physical_property_Color.text = "흰색";
            Physical_property_MeltingPoint.text = "약 205°C (분해점)";
            Physical_property_Solubility.text = "물에 약간 용해됨 (25°C에서 약 8.57 g/L)";
            Physical_property_pKa.text = "a-카복시기: 약 2.10\r\nγ-카복시기: 약 4.07\r\na-아미노기: 약 9.47";

            Chemical_property_a_ArminoAcid.text = "생체 내에서 L-형태로 존재하며, α-탄소에 단일 입체중심이 있음";
            Chemical_property_SideChain.text = "γ-카복시기를 포함하여 pH에서 음전하 → 단백질 기능에 영향";
            Chemical_property_Stereochemistry.text = "카이랄 중심을 가진 L-글루탐산이 단백질 합성에 사용됨";
            Chemical_property_Symmetry.text = "입체중심 하나, 비대칭 분자 구조";

            Exceptions_StereoisomersExist.text = "D형 존재하지만 생체 내 주 기능은 L-Glu → 흥분성 신경전달물질 역할";
            Exceptions_Scanning.text = "글루타민의 전구체로 작용하며 질소대사 실험에서 활용됨";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/글루탐산";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Glutamic-acid";
            RelevantLink3.text = "https://www.nature.com/articles/ejcn2010142";*/
        }
        else if (MBC.NowModelName == "Arginine")
        {
            Basic_property_Name.text = "아르기닌 (Arginine)";
            Basic_property_IUPAC.text = "(2S)-2-아미노-5-구아니디노펜탄산";
            Basic_property_Acronym.text = "Arg 또는 R";
            Basic_property_Classification.text = "조건부 필수 아미노산, 염기성 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>6</sub>H<sub>14</sub>N<sub>4</sub>O<sub>2</sub>";  // Arginine 
            Basic_property_MolecularWeight.text = "174.20 g/mol";

            Physical_property_State.text = "고체 (결정성 또는 분말 형태)";
            Physical_property_Color.text = "흰색";
            Physical_property_MeltingPoint.text = "약 222°C (분해점)";
            Physical_property_Solubility.text = "물에 잘 용해됨 (25°C에서 약 100 mg/mL), 에탄올에는 약간 용해됨";
            Physical_property_pKa.text = "a-카복시기: 약 2.18\r\na-아미노기: 약 9.09\r\n구아니디노기: 약 13.8";

            Chemical_property_a_ArminoAcid.text = "생체 내에서 L-형태로 존재하며, α-탄소에 단일 입체중심이 있음";
            Chemical_property_SideChain.text = "구아니디노기 포함 → 생리적 pH에서 항상 양전하 유지";
            Chemical_property_Stereochemistry.text = "L-아르기닌만 단백질 합성에 사용됨";
            Chemical_property_Symmetry.text = "입체중심 하나 보유, 비대칭 구조";

            Exceptions_StereoisomersExist.text = "L-형만 생체 내에 존재하며, 다양한 대사경로 (NO 생성, 요소회로 등)에 관여";
            Exceptions_Scanning.text = "질산화합물 생성 관련 실험에서 전구체로 사용";
/*
            RelevantLink1.text = "https://ko.wikipedia.org/wiki/아르기닌";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Arginine";
            RelevantLink3.text = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC7572373/";*/
        }
        else if (MBC.NowModelName == "Serine")
        {
            Basic_property_Name.text = "세린 (Serine)";
            Basic_property_IUPAC.text = "(2S)-2-아미노-3-하이드록시프로판산";
            Basic_property_Acronym.text = "Ser 또는 S";
            Basic_property_Classification.text = "비필수 아미노산, 극성 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>3</sub>H<sub>7</sub>NO<sub>3</sub>";  // Serine  
            Basic_property_MolecularWeight.text = "105.09 g/mol";

            Physical_property_State.text = "고체 (결정성 또는 분말 형태)";
            Physical_property_Color.text = "흰색";
            Physical_property_MeltingPoint.text = "약 228°C (분해점)";
            Physical_property_Solubility.text = "물에 잘 용해됨 (25°C에서 약 425 g/L)";
            Physical_property_pKa.text = "a-카복시기: 약 2.19\r\na-아미노기: 약 9.21";

            Chemical_property_a_ArminoAcid.text = "L-형태로 존재하며, α-탄소에 단일 입체중심이 있음";
            Chemical_property_SideChain.text = "곁사슬에 하이드록시메틸기(-CH₂OH) → 극성, 수소결합 형성 가능";
            Chemical_property_Stereochemistry.text = "L-세린만 단백질 합성에 사용되며, D-세린은 신경계에서 기능";
            Chemical_property_Symmetry.text = "비대칭 탄소 중심으로 카이랄 구조";

            Exceptions_StereoisomersExist.text = "D-세린은 NMDA 수용체 보조작용자로 신경 전달에 관여함";
            Exceptions_Scanning.text = "단백질 인산화, 글리코실화 연구에서 하이드록시기 반응성으로 실험 활용";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/세린";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Serine";
            RelevantLink3.text = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC8629603/";*/
        }
        else if (MBC.NowModelName == "Asparagine")
        {
            Basic_property_Name.text = "아스파라긴 (Asparagine)";
            Basic_property_IUPAC.text = "(2S)-2,4-diamino-4-oxobutanoic acid";
            Basic_property_Acronym.text = "Asn 또는 N";
            Basic_property_Classification.text = "극성 아미노산, 비전하성, 친수성, 아미드 곁사슬 보유";
            Basic_property_ChemicalFormula.text = "C<sub>4</sub>H<sub>8</sub>N<sub>2</sub>O<sub>3</sub>";  // Asparagine 
            Basic_property_MolecularWeight.text = "132.12 g/mol";

            Physical_property_State.text = "고체";
            Physical_property_Color.text = "백색 또는 거의 무색";
            Physical_property_MeltingPoint.text = "약 234°C (가열 시 분해됨)";
            Physical_property_Solubility.text = "물에 잘 녹음 (유기 용매에는 거의 불용)";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.13\r\npKa2 (NH₃⁺): 약 8.84\r\n등전점(pI): 약 5.41";

            Chemical_property_a_ArminoAcid.text = "L-형만 단백질 합성에 사용되며, α-탄소에 단일 입체중심 존재";
            Chemical_property_SideChain.text = "아미드기 (-CONH₂)를 가지며 극성이고 수소결합 잘 형성함";
            Chemical_property_Stereochemistry.text = "천연 단백질에는 L-아스파라긴만 존재";
            Chemical_property_Symmetry.text = "카이랄 중심 1개 보유";

            Exceptions_StereoisomersExist.text = "시간 경과에 따라 탈아미드화되어 글루탐산으로 전환될 수 있음";
            Exceptions_Scanning.text = "단백질의 N-글리코실화 수용체 역할로 실험에 활용됨";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/아스파라긴";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Asparagine";
            RelevantLink3.text = "https://pubmed.ncbi.nlm.nih.gov/31182200/";*/
        }
        else if (MBC.NowModelName == "Glutamine")
        {
            Basic_property_Name.text = "글루타민 (Glutamine)";
            Basic_property_IUPAC.text = "(2S)-2,5-diamino-5-oxopentanoic acid";
            Basic_property_Acronym.text = "Gln 또는 Q";
            Basic_property_Classification.text = "극성 아미노산, 비필수 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>5</sub>H<sub>10</sub>N<sub>2</sub>O<sub>3</sub>";  // Glutamine
            Basic_property_MolecularWeight.text = "146.15 g/mol";

            Physical_property_State.text = "고체";
            Physical_property_Color.text = "흰색";
            Physical_property_MeltingPoint.text = "약 185~190°C";
            Physical_property_Solubility.text = "물에 잘 녹음";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.17\r\npKa2 (NH₃⁺): 약 9.13\r\n등전점(pI): 약 5.41";

            Chemical_property_a_ArminoAcid.text = "L-글루타민이 생체 내 주요 형태이며, α-탄소에 아미노기, 카복실기, 곁사슬 보유";
            Chemical_property_SideChain.text = "글루탐산의 γ-카복실기 대신 아미드기(-CONH₂) 보유 → 극성 곁사슬";
            Chemical_property_Stereochemistry.text = "단백질 합성에 사용되는 것은 L-형";
            Chemical_property_Symmetry.text = "입체중심 하나 보유, 비대칭 구조";

            Exceptions_StereoisomersExist.text = "조건부 필수 아미노산으로 외상, 감염 등 스트레스 상황에서 요구량 증가";
            Exceptions_Scanning.text = "질소 운반 및 면역세포/장세포 에너지원, 글루타티온 전구체로 항산화 실험에 사용";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/글루타민";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Glutamine";
            RelevantLink3.text = "https://pubmed.ncbi.nlm.nih.gov/?term=glutamine+metabolism+cancer";*/
        }
        else if (MBC.NowModelName == "Threonine")
        {
            Basic_property_Name.text = "트레오닌 (Threonine)";
            Basic_property_IUPAC.text = "2-아미노-3-하이드록시뷰탄산 (2-Amino-3-hydroxybutanoic acid)";
            Basic_property_Acronym.text = "Thr 또는 T";
            Basic_property_Classification.text = "극성, 비필수 α-아미노산 (일부 조건하에서 필수)";
            Basic_property_ChemicalFormula.text = "C<sub>4</sub>H<sub>9</sub>NO<sub>3</sub>";  // Threonine
            Basic_property_MolecularWeight.text = "119.12 g/mol";

            Physical_property_State.text = "고체 (결정성)";
            Physical_property_Color.text = "백색";
            Physical_property_MeltingPoint.text = "256 °C (분해 동반)";
            Physical_property_Solubility.text = "물에 잘 녹음, 알코올에는 부분 용해";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.09\r\npKa2 (NH₃⁺): 약 9.10\r\n등전점(pI): 약 5.65";

            Chemical_property_a_ArminoAcid.text = "α-탄소에 NH₂, COOH, H, 곁사슬(-CH(OH)CH₃)이 결합된 구조";
            Chemical_property_SideChain.text = "하이드록시기 포함된 치환 알킬기 → 극성, 수소결합 형성 가능";
            Chemical_property_Stereochemistry.text = "2개의 카이랄 중심 → 총 4가지 입체이성질체 존재 (L-threonine이 생체 주형)";
            Chemical_property_Symmetry.text = "2개의 비대칭 탄소 원자를 가진 드문 α-아미노산";

            Exceptions_StereoisomersExist.text = "L-threonine과 L-allothreonine 존재, 생체 내에서는 L-threonine만 주요 사용";
            Exceptions_Scanning.text = "단백질 인산화 부위로 작용 가능 → 신호전달 연구에서 자주 사용됨";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/트레오닌";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Threonine";
            RelevantLink3.text = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC8182025/";*/
        }
        else if (MBC.NowModelName == "Tyrosine")
        {
            Basic_property_Name.text = "타이로신 (Tyrosine)";
            Basic_property_IUPAC.text = "2-아미노-3-(4-하이드록시페닐)프로판산";
            Basic_property_Acronym.text = "Tyr 또는 Y";
            Basic_property_Classification.text = "극성, 조건부 비필수 α-아미노산, 방향족 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>9</sub>H<sub>11</sub>NO<sub>3</sub>";  // Tyrosine
            Basic_property_MolecularWeight.text = "181.19 g/mol";

            Physical_property_State.text = "고체 (결정성)";
            Physical_property_Color.text = "백색 또는 미황색";
            Physical_property_MeltingPoint.text = "343 °C (분해 동반)";
            Physical_property_Solubility.text = "물에 부분적으로 용해, 강염기에는 잘 녹음";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.20\r\npKa2 (NH₃⁺): 약 9.11\r\npKa3 (페놀-OH): 약 10.07\r\n등전점(pI): 약 5.66";

            Chemical_property_a_ArminoAcid.text = "중심 α-탄소에 NH₂, COOH, H, 곁사슬(-CH₂-페놀기) 결합";
            Chemical_property_SideChain.text = "방향족 페놀기 포함 → 극성, 수소결합 및 인산화 가능";
            Chemical_property_Stereochemistry.text = "생체 내에서 L-타이로신만 사용됨";
            Chemical_property_Symmetry.text = "카이랄 중심 하나 보유";

            Exceptions_StereoisomersExist.text = "L-타이로신은 도파민, 노르에피네프린, 멜라닌 등 생합성의 전구체";
            Exceptions_Scanning.text = "단백질 인산화 부위로 작용 → 세포 신호전달 조절 연구에 활용됨";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/타이로신";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Tyrosine";
            RelevantLink3.text = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC9712036/";*/
        }
        else if (MBC.NowModelName == "Cysteine")
        {
            Basic_property_Name.text = "시스테인 (Cysteine)";
            Basic_property_IUPAC.text = "2-아미노-3-설프하이드릴프로판산 (2-Amino-3-sulfhydrylpropanoic acid)";
            Basic_property_Acronym.text = "Cys 또는 C";
            Basic_property_Classification.text = "극성, 조건부 비필수 α-아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>3</sub>H<sub>7</sub>NO<sub>2</sub>S";  // Cysteine
            Basic_property_MolecularWeight.text = "121.16 g/mol";

            Physical_property_State.text = "고체 (결정성)";
            Physical_property_Color.text = "백색";
            Physical_property_MeltingPoint.text = "240 °C (분해 동반)";
            Physical_property_Solubility.text = "물에 잘 녹음, 알코올에는 부분 용해";
            Physical_property_pKa.text = "pKa1 (COOH): 약 1.92\r\npKa2 (NH₃⁺): 약 10.70\r\npKa3 (SH): 약 8.37\r\n등전점(pI): 약 5.07";

            Chemical_property_a_ArminoAcid.text = "중심 α-탄소에 NH₂, COOH, H, 곁사슬(-CH₂SH) 결합";
            Chemical_property_SideChain.text = "티올기(-SH) 포함 → 극성, 환원성, 금속 이온 결합 가능";
            Chemical_property_Stereochemistry.text = "생체 내에서는 L-시스테인만 존재";
            Chemical_property_Symmetry.text = "입체중심 하나 보유";

            Exceptions_StereoisomersExist.text = "이황화결합 형성 → 단백질 3차 구조 안정화에 중요 (시스틴 생성)";
            Exceptions_Scanning.text = "산화-환원 실험 및 글루타티온(GSH) 연구에 활용됨";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/시스테인";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Cysteine";
            RelevantLink3.text = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC9546600/";
*/
        }
        else if (MBC.NowModelName == "Methionine")
        {
            Basic_property_Name.text = "메티오닌 (Methionine)";
            Basic_property_IUPAC.text = "2-Amino-4-(methylthio)butanoic acid";
            Basic_property_Acronym.text = "Met 또는 M";
            Basic_property_Classification.text = "비극성, 황 함유 아미노산, 필수 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>5</sub>H<sub>11</sub>NO<sub>2</sub>S";  // Methionine  
            Basic_property_MolecularWeight.text = "149.21 g/mol";

            Physical_property_State.text = "고체 (결정성)";
            Physical_property_Color.text = "무색 또는 흰색 결정";
            Physical_property_MeltingPoint.text = "약 281°C (분해 수반)";
            Physical_property_Solubility.text = "물에 용해, 에탄올에는 거의 불용성";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.13\r\npKa2 (NH₃⁺): 약 9.28";

            Chemical_property_a_ArminoAcid.text = "리보솜 수준에서 단백질 합성 시작 아미노산 (AUG 코돈)";
            Chemical_property_SideChain.text = "메틸티오기(-SCH₃) 곁사슬 → 비극성이지만 황 원자 포함";
            Chemical_property_Stereochemistry.text = "생체 내에서는 L-메티오닌만 사용됨";
            Chemical_property_Symmetry.text = "입체중심 하나 보유";

            Exceptions_StereoisomersExist.text = "산화되면 메티오닌 설폭사이드로 전환됨 → 산화 스트레스 마커로 사용";
            Exceptions_Scanning.text = "암세포 대사, 수명 연구, 단백질 시작 부위 인식 등 다양한 생리실험에서 활용됨";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/메티오닌";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Methionine";
            RelevantLink3.text = "https://www.nature.com/articles/s41568-019-0187-8";*/

        }
        else if (MBC.NowModelName == "Tryptophan")
        {
            Basic_property_Name.text = "트립토판 (Tryptophan)";
            Basic_property_IUPAC.text = "2-Amino-3-(1H-indol-3-yl)propanoic acid";
            Basic_property_Acronym.text = "Trp 또는 W";
            Basic_property_Classification.text = "극성, 방향족 아미노산, 필수 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>11</sub>H<sub>12</sub>N<sub>2</sub>O<sub>2</sub>";  // Tryptophan 
            Basic_property_MolecularWeight.text = "204.23 g/mol";

            Physical_property_State.text = "고체 (결정성)";
            Physical_property_Color.text = "흰색 또는 미색 결정";
            Physical_property_MeltingPoint.text = "약 289°C (분해 수반)";
            Physical_property_Solubility.text = "물에 약간 용해, 에탄올/에테르에는 불용성";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.38\r\npKa2 (NH₃⁺): 약 9.39";

            Chemical_property_a_ArminoAcid.text = "α-탄소에 아미노기, 카복실기, 인돌기 결합";
            Chemical_property_SideChain.text = "인돌 고리 (6원 벤젠 + 5원 질소 고리) → 방향족, UV 흡수";
            Chemical_property_Stereochemistry.text = "생체 내에서 L-트립토판만 사용됨";
            Chemical_property_Symmetry.text = "입체중심 하나 보유";

            Exceptions_StereoisomersExist.text = "세로토닌, 멜라토닌, 니아신 등 다양한 생체물질의 전구체";
            Exceptions_Scanning.text = "장-뇌 축, 수면, 기분 조절 관련 연구에서 실험적으로 활용됨";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/트립토판";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Tryptophan";
            RelevantLink3.text = "https://pubmed.ncbi.nlm.nih.gov/19650771/";*/

        }
        else if (MBC.NowModelName == "Phenylalanine") 
        {
            Basic_property_Name.text = "페닐알라닌 (Phenylalanine)";
            Basic_property_IUPAC.text = "2-Amino-3-phenylpropanoic acid";
            Basic_property_Acronym.text = "Phe 또는 F";
            Basic_property_Classification.text = "비극성, 방향족 아미노산, 필수 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>9</sub>H<sub>11</sub>NO<sub>2</sub>";  // Phenylalanine  
            Basic_property_MolecularWeight.text = "165.19 g/mol";

            Physical_property_State.text = "고체 (결정성)";
            Physical_property_Color.text = "무색 또는 백색 결정";
            Physical_property_MeltingPoint.text = "약 270°C (분해 수반)";
            Physical_property_Solubility.text = "물에 약간 용해, 알코올에는 거의 불용성";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.20\r\npKa2 (NH₃⁺): 약 9.31";

            Chemical_property_a_ArminoAcid.text = "α-탄소에 아미노기, 카복실기, 벤질기(phenyl group + methylene) 결합";
            Chemical_property_SideChain.text = "벤젠 고리를 포함한 비극성 방향족 곁사슬 → UV 흡수, 소수성";
            Chemical_property_Stereochemistry.text = "생체 내에서는 L-페닐알라닌만 사용됨";
            Chemical_property_Symmetry.text = "입체중심 하나 보유";

            Exceptions_StereoisomersExist.text = "티로신(Tyrosine)의 전구체이며, 페닐케톤뇨증(PKU) 관련 대사 장애 있음";
            Exceptions_Scanning.text = "PKU 연구 및 아미노산 대사 실험에서 사용됨";
/*
            RelevantLink1.text = "https://ko.wikipedia.org/wiki/페닐알라닌";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Phenylalanine";
            RelevantLink3.text = "https://pubmed.ncbi.nlm.nih.gov/18566668/";*/

        }
        else if (MBC.NowModelName == "Isoleucine")
        {
            Basic_property_Name.text = "아이소류신 (Isoleucine)";
            Basic_property_IUPAC.text = "2-Amino-3-methylpentanoic acid";
            Basic_property_Acronym.text = "Ile 또는 I";
            Basic_property_Classification.text = "비극성, 가지사슬 아미노산 (BCAA), 필수 아미노산";
            Basic_property_ChemicalFormula.text = "C<sub>6</sub>H<sub>13</sub>NO<sub>2</sub>";  // Isoleucine 
            Basic_property_MolecularWeight.text = "131.17 g/mol";

            Physical_property_State.text = "고체 (백색 결정성)";
            Physical_property_Color.text = "무색 또는 흰색";
            Physical_property_MeltingPoint.text = "약 284°C (분해 수반)";
            Physical_property_Solubility.text = "물에 용해됨, 알코올에는 불용성 또는 거의 불용성";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.36\r\npKa2 (NH₃⁺): 약 9.60";

            Chemical_property_a_ArminoAcid.text = "곁사슬은 비대칭 가지사슬 형태로 메틸기와 에틸기 포함";
            Chemical_property_SideChain.text = "소수성 가지사슬 → 막 단백질, 단백질 내부에 주로 위치";
            Chemical_property_Stereochemistry.text = "L-이소류신만 생체 내에서 사용됨";
            Chemical_property_Symmetry.text = "입체중심 하나 보유";

            Exceptions_StereoisomersExist.text = "류신과 구조 이성질체이며, 대사 및 근육 에너지 공급에 특화";
            Exceptions_Scanning.text = "근육 합성, 혈당 조절 실험에서 사용됨";

            //RelevantLink1.text = "https://ko.wikipedia.org/wiki/아이소류신";
            //RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Isoleucine";
            //RelevantLink3.text = "https://pubmed.ncbi.nlm.nih.gov/33289736/";

        }
        else if (MBC.NowModelName == "Leucine")
        {
            Basic_property_Name.text = "류신 (Leucine)";
            Basic_property_IUPAC.text = "2-amino-4-methylpentanoic acid";
            Basic_property_Acronym.text = "Leu 또는 L";
            Basic_property_Classification.text = "필수 아미노산, 비극성, 가지사슬 아미노산 (BCAA)";
            Basic_property_ChemicalFormula.text = "C<sub>6</sub>H<sub>13</sub>NO<sub>2</sub>";  // Leucine 
            Basic_property_MolecularWeight.text = "131.17 g/mol";

            Physical_property_State.text = "고체 (백색 결정성)";
            Physical_property_Color.text = "무색 또는 백색";
            Physical_property_MeltingPoint.text = "약 293°C (분해점)";
            Physical_property_Solubility.text = "물에는 약간 용해 (25°C에서 약 2.4 g/100 mL), 에탄올에는 거의 불용";
            Physical_property_pKa.text = "pKa1 (COOH): 약 2.36\r\npKa2 (NH₃⁺): 약 9.60\r\n등전점(pI): 약 6.01";

            Chemical_property_a_ArminoAcid.text = "α-탄소에 NH₂, COOH, H, 곁사슬(isobutyl group) 결합";
            Chemical_property_SideChain.text = "가지형 탄화수소 곁사슬 → 강한 소수성";
            Chemical_property_Stereochemistry.text = "생체 내에서 L-류신만 사용됨";
            Chemical_property_Symmetry.text = "카이랄 중심 하나 보유";

            Exceptions_StereoisomersExist.text = "케토제닉 아미노산으로 에너지 생성 시 케톤체로 전환 가능";
            Exceptions_Scanning.text = "단백질 합성 촉진, 근육 성장 연구 (mTOR signaling)에서 활용됨";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/류신";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Leucine";
            RelevantLink3.text = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC7168362/";*/

        }
        else if (MBC.NowModelName == "Proline")
        {
            Basic_property_Name.text = "프롤린 (Proline)";
            Basic_property_IUPAC.text = "(S)-피롤리딘-2-카복실산 ((S)-Pyrrolidine-2-carboxylic acid)";
            Basic_property_Acronym.text = "Pro 또는 P";
            Basic_property_Classification.text = "비극성, 비필수 α-아미노산, 2차 아민 구조 (특수 구조)";
            Basic_property_ChemicalFormula.text = "C<sub>5</sub>H<sub>9</sub>NO<sub>2</sub>";  // Proline
            Basic_property_MolecularWeight.text = "115.13 g/mol";

            Physical_property_State.text = "고체 (결정성)";
            Physical_property_Color.text = "백색 또는 무색";
            Physical_property_MeltingPoint.text = "약 220~230°C (분해 동반)";
            Physical_property_Solubility.text = "물에 잘 녹음, 에탄올에는 낮은 용해도";
            Physical_property_pKa.text = "pKa1 (COOH): 약 1.95\r\npKa2 (NH₂⁺): 약 10.64\r\n등전점(pI): 약 6.30";

            Chemical_property_a_ArminoAcid.text = "고리 구조의 2차 아민 → 자유회전 불가, 구조 제한적";
            Chemical_property_SideChain.text = "피롤리딘 고리 구조 → 단단하고 유연성 낮음";
            Chemical_property_Stereochemistry.text = "L-프롤린이 생체에서 사용됨";
            Chemical_property_Symmetry.text = "입체중심 하나 보유";

            Exceptions_StereoisomersExist.text = "헬릭스 브레이커로 작용 → α-helix, β-sheet 구조 꺾는 역할";
            Exceptions_Scanning.text = "콜라겐 구조 및 꺾이는 부위 연구에 활용됨";

            /*RelevantLink1.text = "https://ko.wikipedia.org/wiki/프롤린";
            RelevantLink2.text = "https://pubchem.ncbi.nlm.nih.gov/compound/Proline";
            RelevantLink3.text = "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC9359942/";
*/
        }
    }
}
