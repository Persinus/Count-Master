using UnityEngine;
using System.Collections.Generic;
public enum GateSide
{
    Left,
    Right
}

public enum GateOperation
{
    Add,
    Subtract,
    Multiply,
    Divide
}




public class GateTrigger : MonoBehaviour
{
    public int gateGroupID;
    public GateSide gateSide;
    public GateOperation operation;
    public int operationValue = 1;

    private static readonly HashSet<int> activatedGroups = new();
    private static readonly Dictionary<int, List<GateTrigger>> gatesByGroup = new();

    private void Awake()
    {
        // Đăng ký vào group
        if (!gatesByGroup.ContainsKey(gateGroupID))
            gatesByGroup[gateGroupID] = new List<GateTrigger>();

        gatesByGroup[gateGroupID].Add(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activatedGroups.Contains(gateGroupID)) return;

        if (other.CompareTag("Clone_Player"))
        {
            activatedGroups.Add(gateGroupID);
            Debug.Log($"🌀 Clone chạm vào cổng {gateSide} [{operation} {operationValue}]");

            HandleGateEffect();
        }
    }

    void HandleGateEffect()
    {
        var spawner = Singleton<PrettyCloneSpawner>.Instance;

        switch (operation)
        {
            case GateOperation.Add:
                spawner.AddClones(operationValue);
                break;
            case GateOperation.Subtract:
                spawner.RemoveClones(operationValue);
                break;
            case GateOperation.Multiply:
                spawner.MultiplyClones(operationValue);
                break;
            case GateOperation.Divide:
                spawner.DivideClones(operationValue);
                break;
        }
         // **Bổ sung: khi clone đi qua gate, chuyển tất cả clone sang trạng thái chạy**
        spawner.ChangeAnimationState(CloneAnimState.Running);

        // Tắt collider của group này
        if (gatesByGroup.TryGetValue(gateGroupID, out var groupGates))
        {
            foreach (var gate in groupGates)
            {
                var col = gate.GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
        }
    }

    public static void ResetGateState()
    {
        activatedGroups.Clear();
    }

    public static void ClearCache()
    {
        gatesByGroup.Clear();
    }
}
