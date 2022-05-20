//
// Created by Sofus Skovgaard on 03/05/2022.
//

#include "DHTManager.h"

DHT_Unified DHTManager::_dht = DHT_Unified(7, DHT11);

sensors_event_t DHTManager::_event;

void DHTManager::configure() {
    Serial.println(PrintHelper::ts() + "[DHT] Configuration initiated");
    _dht.begin();
}

float DHTManager::read_humidity() {
    _dht.humidity().getEvent(&_event);
    return _event.relative_humidity;
}

float DHTManager::read_temperature() {
    _dht.temperature().getEvent(&_event);
    return _event.temperature;
}
