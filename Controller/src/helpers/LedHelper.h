//
// Created by Sofus Skovgaard on 02/05/2022.
//

#ifndef SMARTHOME_LEDHELPER_H
#define SMARTHOME_LEDHELPER_H

#include "Arduino.h"
#include <WiFiNINA.h>
#include <utility/wifi_drv.h>

class LedHelper {
public:
    static void configure();

    static void led_on();

    static void led_off();

    static void toggle_led();

    static void strobe_led(unsigned long duration);
};


#endif //SMARTHOME_LEDHELPER_H
