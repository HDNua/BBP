#!/bin/bash
sed 's/[(,)]/ /g' input.txt | awk '{print $5, $6, $7}' > default_pallete.txt
gawk '{printf "0x%02X%02X%02X, ", strtonum($1), strtonum($2), strtonum($3)}' default_pallete.txt > code.txt
