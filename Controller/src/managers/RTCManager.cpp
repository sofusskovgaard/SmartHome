//
// Created by Sofus Skovgaard on 03/05/2022.
//

#include "RTCManager.h"

RTC_DS3231 RTCManager::__rtc = RTC_DS3231();

void RTCManager::configure() {
    if (!__rtc.begin()) {
        Serial.println("[ERROR] Couldn't find RTC");
    }
}
