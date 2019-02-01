using System.Collections;
using System.Collections.Generic;
using System;

public class IActionProcessMessage {
    public short cmd;
    public Action<MessageReceiving> functionProcess;
}
