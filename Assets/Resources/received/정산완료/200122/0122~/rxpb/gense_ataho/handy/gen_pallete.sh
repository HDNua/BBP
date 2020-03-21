#!/bin/bash
gawk '{printf "0x%02X%02X%02X, ", $1, $2, $3}' default_pallete.txt

