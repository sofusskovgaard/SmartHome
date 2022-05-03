//
// Created by Sofus Skovgaard on 03/05/2022.
//

#ifndef SMARTHOME_RTCMANAGER_H
#define SMARTHOME_RTCMANAGER_H

#include <RTClib.h>

class RTCManager {
private:
    static RTC_DS3231 __rtc;
public:
    static RTC_DS3231 get_rtc() {
        return __rtc;
    }

    static void configure();
};


#endif //SMARTHOME_RTCMANAGER_H
