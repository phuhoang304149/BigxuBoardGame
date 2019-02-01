using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalRacing_RaceTrackController : MonoBehaviour
{

    public Transform startPoint;
    public Transform finishPoint;
    public List<AnimalRacing_RaceTrack_Col_Controller> listColGlass;
    public List<AnimalRacing_RaceTrack_Col_Controller> listColTree00;
    public List<AnimalRacing_RaceTrack_Col_Controller> listColTree01;

    public bool canMoveReverseCamera { get; set; }


    public void ResetData()
    {
        for (int i = 0; i < listColGlass.Count; i++)
        {
            listColGlass[i].ResetData();
        }
        for (int i = 0; i < listColTree00.Count; i++)
        {
            listColTree00[i].ResetData();
        }
        for (int i = 0; i < listColTree01.Count; i++)
        {
            listColTree01[i].ResetData();
        }
        canMoveReverseCamera = true;
    }

    public void InitData(float _s)
    {
        Vector3 _pos = startPoint.position;
        _pos.x += _s;
        finishPoint.position = _pos;
    }

    AnimalRacing_RaceTrack_Col_Controller GetLastCol(List<AnimalRacing_RaceTrack_Col_Controller> _listCol)
    {
        int index = 0;
        float maxX = _listCol[index].transform.position.x;
        for (int i = 1; i < _listCol.Count; i++)
        {
            if (_listCol[i].transform.position.x > maxX)
            {
                index = i;
                maxX = _listCol[i].transform.position.x;
            }
        }
        return _listCol[index];
    }

    AnimalRacing_RaceTrack_Col_Controller GetFirstCol(List<AnimalRacing_RaceTrack_Col_Controller> _listCol)
    {
        int index = 0;
        float minX = _listCol[index].transform.position.x;
        for (int i = 1; i < _listCol.Count; i++)
        {
            if (_listCol[i].transform.position.x < minX)
            {
                index = i;
                minX = _listCol[i].transform.position.x;
            }
        }
        return _listCol[index];
    }

    public void UpdatePosAgain(float _currentVelocity)
    {
        Vector3 _pos = Vector3.zero;
        MyCameraController _camera = AnimalRacing_GamePlay_Manager.instance.mainCamera;
        AnimalRacing_RaceTrack_Col_Controller _firstCol = null;
        AnimalRacing_RaceTrack_Col_Controller _lastCol = null;

        if (canMoveReverseCamera)
        {
            for (int i = 0; i < listColGlass.Count; i++)
            {
                if (listColGlass[i].percentReverseVeclocity > 0f)
                {
                    _pos = listColGlass[i].transform.position;
                    _pos.x += listColGlass[i].percentReverseVeclocity * _currentVelocity / 100f;
                    listColGlass[i].transform.position = _pos;
                }
            }

            for (int i = 0; i < listColTree00.Count; i++)
            {
                if (listColTree00[i].percentReverseVeclocity > 0f)
                {
                    _pos = listColTree00[i].transform.position;
                    _pos.x += listColTree00[i].percentReverseVeclocity * _currentVelocity / 100f;
                    listColTree00[i].transform.position = _pos;
                }
            }

            for (int i = 0; i < listColTree01.Count; i++)
            {
                if (listColTree01[i].percentReverseVeclocity > 0f)
                {	
                    _pos = listColTree01[i].transform.position;
                    _pos.x += listColTree01[i].percentReverseVeclocity * _currentVelocity / 100f;
                    listColTree01[i].transform.position = _pos;
                }
            }
        }

        _firstCol = GetFirstCol(listColGlass);
        _pos = _firstCol.transform.position;
        if (_pos.x + _firstCol.mySize.x / 2 <= _camera.transform.position.x - _camera.sizeOfCamera.x / 2f)
        {
            _lastCol = GetLastCol(listColGlass);
            _pos.x = _lastCol.transform.position.x + _lastCol.mySize.x;
            _firstCol.transform.position = _pos;
        }

        _firstCol = GetFirstCol(listColTree00);
        _pos = _firstCol.transform.position;
        if (_pos.x + _firstCol.mySize.x / 2 <= _camera.transform.position.x - _camera.sizeOfCamera.x / 2f)
        {
            _lastCol = GetLastCol(listColTree00);
            _pos.x = _lastCol.transform.position.x + _lastCol.mySize.x;
            _firstCol.transform.position = _pos;
        }

        _firstCol = GetFirstCol(listColTree01);
        _pos = _firstCol.transform.position;
        if (_pos.x + _firstCol.mySize.x / 2 <= _camera.transform.position.x - _camera.sizeOfCamera.x / 2f)
        {
            _lastCol = GetLastCol(listColTree01);
            _pos.x = _lastCol.transform.position.x + _lastCol.mySize.x;
            _firstCol.transform.position = _pos;
        }
    }
}
