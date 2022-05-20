//
// Created by Sofus Skovgaard on 03/05/2022.
//

#ifndef SMARTHOME_SERVOMANAGER_H
#define SMARTHOME_SERVOMANAGER_H

#include <Servo.h>

class ServoManager {
private:
    static Servo _servo;
    static int _pos;

public:
    static void open();

    static void close();

    static void toggle();

    static void configure();
};


#endif //SMARTHOME_SERVOMANAGER_H
