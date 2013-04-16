// For Intellisense
#ifdef _MSC_VER
#	define __AVR_ATmega328P__
#	include <Arduino.h>
#endif

void setup()
{
	pinMode(A0, INPUT);
	analogReference(INTERNAL);

	Serial.begin(2000000);
	Serial.write('G');
}

void loop() { }

void serialEvent()
{
	if (Serial.read() == 'R')
	{
		uint16_t v = analogRead(0);
		Serial.write(v);
		Serial.write(v >> 8);
	}
}
