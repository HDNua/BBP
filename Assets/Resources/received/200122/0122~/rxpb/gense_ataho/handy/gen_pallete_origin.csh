#!/bin/bash
gawk '{printf "%02X%02X%02X\n", $1, $2, $3}' default_pallete.txt

