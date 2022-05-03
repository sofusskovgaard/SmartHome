//
// Created by Sofus Skovgaard on 03/05/2022.
//

#ifndef SMARTHOME_SENDERCOMMANDBASE_H
#define SMARTHOME_SENDERCOMMANDBASE_H

#include <string>

#include "managers/RTCManager.h"

class SenderCommandBase {
public:
    SenderCommandBase();
    virtual ~SenderCommandBase() {}

    DateTime collected_at;

    virtual std::string topic();
};


#endif //SMARTHOME_SENDERCOMMANDBASE_H
