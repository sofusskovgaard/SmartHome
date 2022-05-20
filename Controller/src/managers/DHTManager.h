//
// Created by Sofus Skovgaard on 03/05/2022.
//

#ifndef SMARTHOME_DHTMANAGER_H
#define SMARTHOME_DHTMANAGER_H

#include <string>
#include <Adafruit_Sensor.h>
#include <DHT.h>
#include <DHT_U.h>

#include "helpers/PrintHelper.h"

class DHTManager {
private:
    static DHT_Unified _dht;
    static sensors_event_t _event;

public:
    static void configure();

    static float read_temperature();

    static float read_humidity();
};


#endif //SMARTHOME_DHTMANAGER_H
