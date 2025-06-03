using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Ԃт�̃Q�[�W
/// </summary>
public class Flower_gauge : MonoBehaviour
{
    public Slider petalGauge;//�Ԃт�Q�[�W
    public int petalsToHeal = 5;//�T���ŉ�
    private int currentPetals = 0;//���݂̉Ԃт琔

    void start()
    {
        //�X���C�_�[�̏����ݒ�
        petalGauge.maxValue = petalsToHeal;
        petalGauge.value = currentPetals;
    }

    void Update()
    {
        if ()//�Ԃт�̎擾
        {
            CollectPetal();
        }
    }
    public void CollectPetal()
    {
        currentPetals++;
        if (currentPetals >= petalsToHeal)//�Ԃт���T�����߂�
        {
            currentPetals = 0;//�Q�[�W�̃��Z�b�g
        }

        petalGauge.value = currentPetals;//�Q�[�WUI�̍X�V
    }
}
