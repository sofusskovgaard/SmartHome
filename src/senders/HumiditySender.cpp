//
// Created by Sofus Skovgaard on 03/05/2022.
//

#include "HumiditySender.h"

std::string HumiditySender::topic = HUMIDITY_SENDER_TOPIC;

std::string HumiditySender::Handle(std::shared_ptr<SenderCommandBase> command) {
    std::shared_ptr<HumiditySenderCommand> _command = std::static_pointer_cast<HumiditySenderCommand>(command);
    return std::to_string(_command->humidity);
}