using UnityEngine;

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
    public GateSide gateSide;
    public GateOperation operation;      // 👉 Phép toán
    public int operationValue = 1;       // 👉 Giá trị áp dụng

    private bool gateActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (gateActivated) return;

        if (other.CompareTag("Clone_Player"))
        {
            gateActivated = true;

            Debug.Log($"🌀 Clone chạm vào cổng {gateSide} [{operation} {operationValue}]");

            HandleGateEffect(other.transform);
        }
    }

    void HandleGateEffect(Transform clone)
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

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}
