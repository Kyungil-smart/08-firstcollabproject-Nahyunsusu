using UnityEngine;

public class EnhanceQuestionTooltip : TooltipComponent
{
    public override string GetTooltipText()
    {
        return "해당 창에선 소지중인 장비를 \r\n강화할 수 있습니다.\r\n\r\n소지중인 장비를 클릭하면 \r\n강화대에 장비가 등록됩니다.\r\n\r\n장비를 등록한 후 [홀], [짝] 버튼을 클릭하면 \r\n[강화 진행] 버튼이 활성화 됩니다.\r\n\r\n[강화 진행] 시, 총 3개의 주사위가 굴러갑니다.\r\n\r\n3개의 주사위의 눈의 합이 조건보다 크고, \r\n[홀], [짝]이 일치하면 강화가 성공합니다.\r\n\r\n두 가지 조건 중 하나 이상이 충족되지 않을 \r\n경우, 강화가 실패합니다.\r\n\r\n강화 성공 여부에 상관 없이 [강화 진행] 시 \r\n일정량의 골드가 소모됩니다.";
    }
}
