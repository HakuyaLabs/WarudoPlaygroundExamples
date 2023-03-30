using UnityEngine;
using Warudo.Core.Attributes;
using Warudo.Core.Graphs;
using Warudo.Core.Scenes;
using Warudo.Plugins.Core.Assets;
using Warudo.Plugins.Core.Assets.Cinematography;

[NodeType(
    Id = "e4b58138-53ed-4ccd-b1a3-6ed89089ddc4", // Must be unique. Generate one at https://guidgenerator.com/
    Title = "Example Node",
    Category ="My Category")]
public class ExampleNode : Node {

    /* DATA INPUTS */
    // Must be public fields.
    // Technically all types are supported, but only serializable types will be stored on scene save.
    // Also, only selected value & reference types can be edited on the editor UI.

    [DataInput] // Any serializable field annotated with [DataInput] will be shown on the editor UI
    public int MyInteger;

    [DataInput]
    public bool Cool = true; // You can assign a default value

    [DataInput]
    [HiddenIf(nameof(Cool), Is.False)] // This field is hidden if Cool == false
    public Vector3 CoolVector;

    [DataInput]
    [HiddenIf(nameof(ShouldHideCoolString))] // Alternatively, you can also specify a method that returns a boolean
    public string CoolString = "I am cool!";

    // Note the method visibility must not be private (may cause problems)
    protected bool ShouldHideCoolString() {
        return !Cool;
    }

    // If you want to disable a data input instead, there is DisabledIf.

    // Asset data inputs are specially handled. A dropdown will be shown on the UI to select the desired asset.
    [DataInput]
    public Asset SomeAsset;

    // You can specify the exact asset type.
    [DataInput]
    public CameraAsset SomeCameraAsset;

    // GameObjectAsset is an asset type that many core asset types inherit from, such as CameraAsset, AnchorAsset, CharacterAsset, etc. If you can see a transform gizmo, it is a GameObjectAsset.
    [DataInput]
    public GameObjectAsset SomeGameObjectAsset;

    /* DATA OUTPUTS */
    // Must be public methods with a return value.

    [DataOutput]
    [Label("My Integer + 1")] // You can customize the port label
    public int MyIntegerPlusOne() {
        return MyInteger + 1; 
    }

    /* FLOW INPUTS */
    // Must be public methods with return type Continuation.

    // Usually, we name the default flow input "Enter". You are of course free to name a flow input differently.
    [FlowInput]
    public Continuation Enter() {
        // We invoke flow on either exit based on the value of the "Cool" data input. Isn't that cool?
        if (Cool) {
            return Exit;
        } else {
            return NotCoolExit;
        }
    }

    [FlowInput]
    public Continuation DoNothing() {
        // You can terminate the flow by returning null
        return null;
    }

    [FlowInput]
    public Continuation InvokeBothExits() {
        // You can also manually invoke a new flow like this
        InvokeFlow(nameof(Exit));
        InvokeFlow(nameof(NotCoolExit));
        return null;
    }

    [FlowInput]
    public Continuation UpdateMyInteger() {
        // Since data inputs are just fields, you can assign values to them directly
        MyInteger = Random.Range(1, 100);

        // If you want to trigger watcher changes (see OnCreate), use the SetDataInput method
        SetDataInput(nameof(MyInteger), 42);

        // To show the updated value on the UI, you must call BroadcastDataInput
        BroadcastDataInput(nameof(MyInteger));
        return null;
    }


    /* FLOW OUTPUTS */
    // Must be public fields with type Continuation without an initial value.

    // Usually, we name the default flow input "Exit".
    [FlowOutput]
    public Continuation Exit;

    [FlowOutput]
    public Continuation NotCoolExit;

    /* LIFECYCLE EVENTS */

    // Every node instance is created exactly once (when dragged onto the graph, or when deserialized on scene load)
    protected override void OnCreate()
    {
        // You can watch value changes and get notified
        Watch(nameof(MyInteger), () => {
            Debug.Log("MyInteger has changed!");
        });
        // If you want to check the previous value, use the generic-typed method
        Watch<int>(nameof(MyInteger), (from, to) => {
            Debug.Log($"It has changed from {from} to {to}.");
        });
        // Get notified when an asset has become active or inactive
        WatchAssetState(nameof(SomeGameObjectAsset), state => {
            Debug.Log($"SomeGameObjectAsset has become {(state ? "Active" : "Inactive")}");
        });
        // WatchAsset watches both the value change and the asset's state change
        WatchAsset(nameof(SomeGameObjectAsset), () => {
            Debug.Log($"SomeGameObjectAsset has changed, or its state is updated");
        });
    }

    // Called when a node is deleted or unloaded from the scene
    protected override void OnDestroy()
    {
    }

    // This is called every frame
    public override void OnUpdate() {
    }

    // There are also OnLateUpdate(), OnEndOfFrame(), etc.

}