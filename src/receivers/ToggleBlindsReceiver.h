//
// Created by Sofus Skovgaard on 03/05/2022.
//

#ifndef SMARTHOME_TOGGLEBLINDSRECEIVER_H
#define SMARTHOME_TOGGLEBLINDSRECEIVER_H

#include <string>

#include "managers/ServoManager.h"

#include "helpers/PrintHelper.h"

class ToggleBlindsReceiver {
public:
    static std::string topic;

    static void Handle(std::string payload);
};


#endif //SMARTHOME_TOGGLEBLINDSRECEIVER_H
