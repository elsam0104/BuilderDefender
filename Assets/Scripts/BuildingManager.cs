using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    public event EventHandler<OnActiveBuildingTypeEventArgs> OnActiveBuildingTypeChanged;

    public class OnActiveBuildingTypeEventArgs :EventArgs {
        public BuildingTypeSO buildingType;
    }


    [SerializeField] private Building hqBuilding;
    private BuildingTypeListSO buildingTypeList;
    private BuildingTypeSO activeBuildingType;
    private Camera mainCamera;
    private void Awake()
    {
        Instance = this;
        buildingTypeList = (BuildingTypeListSO)Resources.Load(typeof(BuildingTypeListSO).Name);

    }
    private void Start()
    {
        mainCamera = Camera.main;

        hqBuilding.GetComponent<HealthSystem>().OnDied += HQ_OnDied;
        
    }

    private void HQ_OnDied(object sender, EventArgs e)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.GameOver);
        GameOverUI.Instance.Show();

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)&&!EventSystem.current.IsPointerOverGameObject())
        {
            if (activeBuildingType != null)
                if (CanSpawnBuilding(activeBuildingType, UtilClass.GetMouseWorldPosition(), out string errorMessage))
                {
                    if (ResourceManager.Instance.CanAfford(activeBuildingType.constructionResourceCostArray))
                    {
                        ResourceManager.Instance.SpendResources(activeBuildingType.constructionResourceCostArray);
                        //Instantiate(activeBuildingType.prefab, UtilClass.GetMouseWorldPosition(), Quaternion.identity);
                        BuildingConstruction.Create(UtilClass.GetMouseWorldPosition(),activeBuildingType);
                        SoundManager.Instance.PlaySound(SoundManager.Sound.BuildingPlaced);
                    }
                    else
                    {
                        TooltipUI.Instance.Show("자원 부족 : " + activeBuildingType.GetConstructioonResourceCostString(),new TooltipUI.TooltipTimer { timer = 2f });
                    }
                }
                else
                {
                    TooltipUI.Instance.Show(errorMessage, new TooltipUI.TooltipTimer { timer = 2f });
                }
        
        }
    }

    public void SetActiveBuildingType(BuildingTypeSO buildingType)
    {
        activeBuildingType = buildingType;
        OnActiveBuildingTypeChanged?.Invoke(this, new OnActiveBuildingTypeEventArgs { buildingType = activeBuildingType }); ;
    }

    public BuildingTypeSO GetActiveBuildingType()
    {
        return activeBuildingType;
    }

    private bool CanSpawnBuilding(BuildingTypeSO buildingtype, Vector3 position, out string errorMessage)
    {
        BoxCollider2D boxCollider2D = buildingtype.prefab.GetComponent<BoxCollider2D>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(position + (Vector3)boxCollider2D.offset, boxCollider2D.size, 0);

        bool isAreaClear = collider2DArray.Length == 0;
        if (!isAreaClear)
        {
            errorMessage = "건물을 놓을 수 없는 곳입니다.";
            return false;
        }
        collider2DArray = Physics2D.OverlapCircleAll(position, buildingtype.minConstructionRadius);

        foreach(Collider2D collider2D in collider2DArray)
        {
            BuildingTypeHolder buildingTypeHolder = collider2D.GetComponent<BuildingTypeHolder>();
            if (buildingTypeHolder != null)
            {
                if (buildingTypeHolder.buildingType == buildingtype)
                {
                    errorMessage = "같은 유형의 건물이 주변에 있습니다.";
                    return false;
                }
            }
        }
        if (buildingtype.hasResourceGeneratorData)
        {
            ResourceGeneratorData resourceGeneratorData = buildingtype.resourceGeneratorData;
            int nearbyResourceAmount = ResourceGenerator.GetNearbyResourceAmount(resourceGeneratorData, position);

            if (nearbyResourceAmount == 0)
            {
                errorMessage = "근처에 리소스 노드가 없습니다.";
                return false;
            }
        }

        float maxContructionRadius = 25f;
        collider2DArray = Physics2D.OverlapCircleAll(position, maxContructionRadius);

        foreach (Collider2D collider2D in collider2DArray)
        {
            BuildingTypeHolder buildingTypeHolder = collider2D.GetComponent<BuildingTypeHolder>();
            if (buildingTypeHolder != null)
            {
                errorMessage = "";
                return true;
            }
        }
        errorMessage = "다른 건물이 주변에 있어야 합니다.";
        return false;
    }

    public Building GetHqBuilding()
    {
        return hqBuilding;
    }
}
