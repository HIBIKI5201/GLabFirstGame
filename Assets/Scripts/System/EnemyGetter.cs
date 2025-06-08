using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// �X�e�[�W�����ׂĂ̓G�̎Q�Ƃ��擾����
/// </summary>
public class EnemyGetter : MonoBehaviour
{
    /// <summary>
    /// �G�̎Q��
    /// </summary>
    public List<Enemy> Enemies => _enemies; // �����_�� �v���p�e�B
    private List<Enemy> _enemies = new List<Enemy>();

    private void Start()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy"); // �V�[�����̃G�l�~�[�̎Q�Ƃ��擾
        foreach(GameObject enemy in enemies)
        {
            _enemies.Add(enemy.GetComponent<Enemy>());�@// �擾�����Q�[���I�u�W�F�N�g����Enemy�N���X���擾���ă��X�g�ɒǉ�����
        }
    }

    /// <summary>
    /// ���X�g����G�l�~�[�̎Q�Ƃ���菜��
    /// �G�����S�����^�C�~���O�ŌĂяo��
    /// </summary>
    public void RemoveEnemy(Enemy enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy); // ���X�g���ɎQ�Ƃ�����΁A���X�g�����������
        }
    }
}
