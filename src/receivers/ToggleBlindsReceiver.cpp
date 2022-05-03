//
// Created by Sofus Skovgaard on 03/05/2022.
//

#include "ToggleBlindsReceiver.h"

std::string ToggleBlindsReceiver::topic = "house/toggle_blinds";

void ToggleBlindsReceiver::Handle(std::string payload) {
    Serial.println(PrintHelper::ts() + " Payload: " + payload.c_str());
    ServoManager::toggle();
}


