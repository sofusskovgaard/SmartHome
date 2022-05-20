//
// Created by Sofus Skovgaard on 03/05/2022.
//

#ifndef SMARTHOME_PRINTHELPER_H
#define SMARTHOME_PRINTHELPER_H

#include <Arduino.h>
#include "RTClib.h"

#include "managers/RTCManager.h"

class PrintHelper {
public:
    static String ts();
};


#endif //SMARTHOME_PRINTHELPER_H
