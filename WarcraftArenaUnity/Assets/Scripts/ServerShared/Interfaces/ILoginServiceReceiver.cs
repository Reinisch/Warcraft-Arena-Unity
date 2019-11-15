using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoginServiceReceiver
{
    void OnJoin(int accountId);
}

