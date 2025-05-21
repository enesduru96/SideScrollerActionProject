using UnityEngine;


/// <summary>
/// This struct contains all the inputs actions available for the character to interact with.
/// </summary>
[System.Serializable]
public struct CharacterActions
{

    // Bool actions
    [Header("Combat")]
    public BoolAction lightAttack;
    public BoolAction drawSheathe;
    public BoolAction block;

    [Header("Interaction")]
    public BoolAction interact;
    public BoolAction interactLite;

    [Header("Movement")]
    public BoolAction jump;
    public BoolAction dash;
    public BoolAction run;
    public BoolAction crouch;
    public Vector2Action movement;

    [Header("Other")]
    public BoolAction jetPack;

    public FloatAction pitch;
    public FloatAction roll;

    public static CharacterActions CreateDefaultActions()
    {
        var actions = new CharacterActions();
        actions.InitializeActions();
        return actions;
    }

    /// <summary>
    /// Reset all the actions.
    /// </summary>
    public void Reset()
    {
        interactLite.Reset();
        lightAttack.Reset();
        drawSheathe.Reset();
        block.Reset();

        jump.Reset();
        run.Reset();
        interact.Reset();
        jetPack.Reset();
        dash.Reset();
        crouch.Reset();

        pitch.Reset();
        roll.Reset();

        movement.Reset();
    }

    /// <summary>
    /// Initializes all the actions by instantiate them. Each action will be instantiated with its specific type (Bool, Float or Vector2).
    /// </summary>
    public void InitializeActions()
    {
        interactLite = new BoolAction();
        interactLite.Initialize();

        lightAttack = new BoolAction();
        lightAttack.Initialize();

        drawSheathe = new BoolAction();
        drawSheathe.Initialize();

        block = new BoolAction();
        block.Initialize();


        jump = new BoolAction();
        jump.Initialize();

        run = new BoolAction();
        run.Initialize();

        interact = new BoolAction();
        interact.Initialize();

        jetPack = new BoolAction();
        jetPack.Initialize();

        dash = new BoolAction();
        dash.Initialize();

        crouch = new BoolAction();
        crouch.Initialize();


        pitch = new FloatAction();
        roll = new FloatAction();

        movement = new Vector2Action();

    }

    /// <summary>
    /// Updates the values of all the actions based on the current input handler (human).
    /// </summary>
    public void SetValues(InputHandler inputHandler)
    {
        if (inputHandler == null)
            return;

        lightAttack.value = inputHandler.GetBool("LightAttack");
        interactLite.value = inputHandler.GetBool("Interact Lite");
        drawSheathe.value = inputHandler.GetBool("DrawSheathe");
        block.value = inputHandler.GetBool("Block");

        jump.value = inputHandler.GetBool("Jump");
        run.value = inputHandler.GetBool("Run");
        interact.value = inputHandler.GetBool("Interact");
        jetPack.value = inputHandler.GetBool("Jet Pack");
        dash.value = inputHandler.GetBool("Dash");
        crouch.value = inputHandler.GetBool("Crouch");

        pitch.value = inputHandler.GetFloat("Pitch");
        roll.value = inputHandler.GetFloat("Roll");

        movement.value = inputHandler.GetVector2("Movement");
    }

    /// <summary>
    /// Copies the values of all the actions from an existing set of actions.
    /// </summary>
    public void SetValues(CharacterActions characterActions)
    {
        lightAttack.value = characterActions.lightAttack.value;
        interactLite.value = characterActions.interactLite.value;
        drawSheathe.value = characterActions.drawSheathe.value;
        block.value = characterActions.block.value;

        jump.value = characterActions.jump.value;
        run.value = characterActions.run.value;
        interact.value = characterActions.interact.value;
        jetPack.value = characterActions.jetPack.value;
        dash.value = characterActions.dash.value;
        crouch.value = characterActions.crouch.value;

        pitch.value = characterActions.pitch.value;
        roll.value = characterActions.roll.value;

        pitch.value = characterActions.pitch.value;
        roll.value = characterActions.roll.value;
        movement.value = characterActions.movement.value;

    }

    /// <summary>
    /// Update all the actions internal states.
    /// </summary>
    public void Update(float dt)
    {
        movement.Update(dt);

        lightAttack.Update(dt);
        drawSheathe.Update(dt);
        interactLite.Update(dt);
        block.Update(dt);

        jump.Update(dt);
        run.Update(dt);
        interact.Update(dt);
        jetPack.Update(dt);
        dash.Update(dt);
        crouch.Update(dt);

    }


}
