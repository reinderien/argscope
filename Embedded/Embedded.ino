// For Intellisense
#ifdef _MSC_VER
#	define __AVR_ATmega328P__
#	include <Arduino.h>
#endif

void setup()
{
	pinMode(A0, INPUT);
	pinMode(A1, INPUT);
	pinMode(A2, INPUT);
	pinMode(A3, INPUT);
	pinMode(A4, INPUT);
	pinMode(A5, INPUT);

	analogReference(INTERNAL);

	Serial.begin(2000000);
}

void loop()
{
}