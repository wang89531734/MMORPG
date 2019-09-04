public class UIStage : BaseUIStage<UIStagePreparation, Stage>
{
    public UIStagePreparation uiStagePreparation;
    public override UIStagePreparation StagePreparation
    {
        get { return uiStagePreparation; }
    }
}
