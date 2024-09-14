using UnityEditor.U2D.Animation;
using UnityEngine;
public class BackgroundScroll : MonoBehaviour
{
    [Tooltip("�w�i��X���̑傫��")] float _length;
    [Tooltip("�ŏ��̍��W")]float _startpos;
    /// <summary> �X�N���[�������鑬��</summary>
    [Header("�X�N���[���̑���")]
    [SerializeField] float _scrollSpeed;
    [SerializeField, Tooltip("�X�N���[��������w�i�f��")] GameObject _background;
    SpriteRenderer _backgroundSprite;
    /// <summary>�w�i�̕���</summary>
    SpriteRenderer _backgroundClone1;
    /// <summary>�w�i�̕���</summary>
    SpriteRenderer _backgroundClone2;

    void Start()
    {
        //�X�s�[�h��+-�����킹��
        _scrollSpeed *= -1;

        // �w�i�摜��x���W
        _startpos = _background.transform.position.x;

        _backgroundSprite = _background.GetComponent<SpriteRenderer>();
        // �w�i�摜��x�������̕�
        _length = _backgroundSprite.GetComponent<SpriteRenderer>().bounds.size.x;

        //�w�i�����E�ɕ������Ĕw�i�I�u�W�F�N�g�̎q�I�u�W�F�N�g�ɂ���
        _backgroundClone1 = Instantiate(_backgroundSprite);
        _backgroundClone1.transform.Translate(-_length, 0, 0);
        _backgroundClone1.transform.SetParent(_backgroundSprite.transform);
        _backgroundClone2 = Instantiate(_backgroundSprite);
        _backgroundClone2.transform.Translate(_length, 0, 0);
        _backgroundClone2.transform.SetParent(_backgroundSprite.transform);

    }
    private void FixedUpdate()
    {
        // �����X�N���[���Ɏg�p����p�����[�^�[
        float temp = (Camera.main.transform.position.x * (1 - _scrollSpeed));
        // �w�i�̎������ʂɎg�p����p�����[�^�[
        float dist = (Camera.main.transform.position.x * _scrollSpeed);
        // �������ʂ�^���鏈��
        // �w�i�摜��x���W��dist�̕��ړ�������
        _background.transform.position = new Vector3(_startpos + dist, _background.transform.position.y, _background.transform.position.z);
        // �����X�N���[��
        // ��ʊO�ɂȂ�����w�i�摜���ړ�������
        if (temp > _startpos + _length) _startpos += _length;
        else if (temp < _startpos - _length) _startpos -= _length;
    }
}