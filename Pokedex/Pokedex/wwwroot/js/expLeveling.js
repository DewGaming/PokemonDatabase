var expGroupId = 0, minimumLevel = 1, maximumExpPoints = 1640000;
var erraticMatrix = [
    { 'Level': 1, 'Exp': 0 },
    { 'Level': 2, 'Exp': 15 },
    { 'Level': 3, 'Exp': 52 },
    { 'Level': 4, 'Exp': 122 },
    { 'Level': 5, 'Exp': 237 },
    { 'Level': 6, 'Exp': 406 },
    { 'Level': 7, 'Exp': 637 },
    { 'Level': 8, 'Exp': 942 },
    { 'Level': 9, 'Exp': 1326 },
    { 'Level': 10, 'Exp': 1800 },
    { 'Level': 11, 'Exp': 2369 },
    { 'Level': 12, 'Exp': 3041 },
    { 'Level': 13, 'Exp': 3822 },
    { 'Level': 14, 'Exp': 4719 },
    { 'Level': 15, 'Exp': 5737 },
    { 'Level': 16, 'Exp': 6881 },
    { 'Level': 17, 'Exp': 8155 },
    { 'Level': 18, 'Exp': 9564 },
    { 'Level': 19, 'Exp': 11111 },
    { 'Level': 20, 'Exp': 12800 },
    { 'Level': 21, 'Exp': 14632 },
    { 'Level': 22, 'Exp': 16610 },
    { 'Level': 23, 'Exp': 18737 },
    { 'Level': 24, 'Exp': 21012 },
    { 'Level': 25, 'Exp': 23437 },
    { 'Level': 26, 'Exp': 26012 },
    { 'Level': 27, 'Exp': 28737 },
    { 'Level': 28, 'Exp': 31610 },
    { 'Level': 29, 'Exp': 34632 },
    { 'Level': 30, 'Exp': 37800 },
    { 'Level': 31, 'Exp': 41111 },
    { 'Level': 32, 'Exp': 44564 },
    { 'Level': 33, 'Exp': 48155 },
    { 'Level': 34, 'Exp': 51881 },
    { 'Level': 35, 'Exp': 55737 },
    { 'Level': 36, 'Exp': 59719 },
    { 'Level': 37, 'Exp': 63822 },
    { 'Level': 38, 'Exp': 68041 },
    { 'Level': 39, 'Exp': 72369 },
    { 'Level': 40, 'Exp': 76800 },
    { 'Level': 41, 'Exp': 81326 },
    { 'Level': 42, 'Exp': 85942 },
    { 'Level': 43, 'Exp': 90637 },
    { 'Level': 44, 'Exp': 95406 },
    { 'Level': 45, 'Exp': 100237 },
    { 'Level': 46, 'Exp': 105122 },
    { 'Level': 47, 'Exp': 110052 },
    { 'Level': 48, 'Exp': 115015 },
    { 'Level': 49, 'Exp': 120001 },
    { 'Level': 50, 'Exp': 125000 },
    { 'Level': 51, 'Exp': 131324 },
    { 'Level': 52, 'Exp': 137795 },
    { 'Level': 53, 'Exp': 144410 },
    { 'Level': 54, 'Exp': 151165 },
    { 'Level': 55, 'Exp': 158056 },
    { 'Level': 56, 'Exp': 165079 },
    { 'Level': 57, 'Exp': 172229 },
    { 'Level': 58, 'Exp': 179503 },
    { 'Level': 59, 'Exp': 186894 },
    { 'Level': 60, 'Exp': 194400 },
    { 'Level': 61, 'Exp': 202013 },
    { 'Level': 62, 'Exp': 209728 },
    { 'Level': 63, 'Exp': 217540 },
    { 'Level': 64, 'Exp': 225443 },
    { 'Level': 65, 'Exp': 233431 },
    { 'Level': 66, 'Exp': 241496 },
    { 'Level': 67, 'Exp': 249633 },
    { 'Level': 68, 'Exp': 257834 },
    { 'Level': 69, 'Exp': 267406 },
    { 'Level': 70, 'Exp': 276458 },
    { 'Level': 71, 'Exp': 286328 },
    { 'Level': 72, 'Exp': 296358 },
    { 'Level': 73, 'Exp': 305767 },
    { 'Level': 74, 'Exp': 316074 },
    { 'Level': 75, 'Exp': 326531 },
    { 'Level': 76, 'Exp': 336255 },
    { 'Level': 77, 'Exp': 346965 },
    { 'Level': 78, 'Exp': 357812 },
    { 'Level': 79, 'Exp': 367807 },
    { 'Level': 80, 'Exp': 378880 },
    { 'Level': 81, 'Exp': 390077 },
    { 'Level': 82, 'Exp': 400293 },
    { 'Level': 83, 'Exp': 411686 },
    { 'Level': 84, 'Exp': 423190 },
    { 'Level': 85, 'Exp': 433572 },
    { 'Level': 86, 'Exp': 445239 },
    { 'Level': 87, 'Exp': 457001 },
    { 'Level': 88, 'Exp': 467489 },
    { 'Level': 89, 'Exp': 479378 },
    { 'Level': 90, 'Exp': 491346 },
    { 'Level': 91, 'Exp': 501878 },
    { 'Level': 92, 'Exp': 513934 },
    { 'Level': 93, 'Exp': 526049 },
    { 'Level': 94, 'Exp': 536557 },
    { 'Level': 95, 'Exp': 548720 },
    { 'Level': 96, 'Exp': 560922 },
    { 'Level': 97, 'Exp': 571333 },
    { 'Level': 98, 'Exp': 583539 },
    { 'Level': 99, 'Exp': 591882 },
    { 'Level': 100, 'Exp': 600000 }
], fastMatrix = [
    { 'Level': 1, 'Exp': 0 },
    { 'Level': 2, 'Exp': 6 },
    { 'Level': 3, 'Exp': 21 },
    { 'Level': 4, 'Exp': 51 },
    { 'Level': 5, 'Exp': 100 },
    { 'Level': 6, 'Exp': 172 },
    { 'Level': 7, 'Exp': 274 },
    { 'Level': 8, 'Exp': 409 },
    { 'Level': 9, 'Exp': 583 },
    { 'Level': 10, 'Exp': 800 },
    { 'Level': 11, 'Exp': 1064 },
    { 'Level': 12, 'Exp': 1382 },
    { 'Level': 13, 'Exp': 1757 },
    { 'Level': 14, 'Exp': 2195 },
    { 'Level': 15, 'Exp': 2700 },
    { 'Level': 16, 'Exp': 3276 },
    { 'Level': 17, 'Exp': 3930 },
    { 'Level': 18, 'Exp': 4665 },
    { 'Level': 19, 'Exp': 5487 },
    { 'Level': 20, 'Exp': 6400 },
    { 'Level': 21, 'Exp': 7408 },
    { 'Level': 22, 'Exp': 8518 },
    { 'Level': 23, 'Exp': 9733 },
    { 'Level': 24, 'Exp': 11059 },
    { 'Level': 25, 'Exp': 12500 },
    { 'Level': 26, 'Exp': 14060 },
    { 'Level': 27, 'Exp': 15746 },
    { 'Level': 28, 'Exp': 17561 },
    { 'Level': 29, 'Exp': 19511 },
    { 'Level': 30, 'Exp': 21600 },
    { 'Level': 31, 'Exp': 23832 },
    { 'Level': 32, 'Exp': 26214 },
    { 'Level': 33, 'Exp': 28749 },
    { 'Level': 34, 'Exp': 31443 },
    { 'Level': 35, 'Exp': 34300 },
    { 'Level': 36, 'Exp': 37324 },
    { 'Level': 37, 'Exp': 40522 },
    { 'Level': 38, 'Exp': 43897 },
    { 'Level': 39, 'Exp': 47455 },
    { 'Level': 40, 'Exp': 51200 },
    { 'Level': 41, 'Exp': 55136 },
    { 'Level': 42, 'Exp': 59270 },
    { 'Level': 43, 'Exp': 63605 },
    { 'Level': 44, 'Exp': 68147 },
    { 'Level': 45, 'Exp': 72900 },
    { 'Level': 46, 'Exp': 77868 },
    { 'Level': 47, 'Exp': 83058 },
    { 'Level': 48, 'Exp': 88473 },
    { 'Level': 49, 'Exp': 94119 },
    { 'Level': 50, 'Exp': 100000 },
    { 'Level': 51, 'Exp': 106120 },
    { 'Level': 52, 'Exp': 112486 },
    { 'Level': 53, 'Exp': 119101 },
    { 'Level': 54, 'Exp': 125971 },
    { 'Level': 55, 'Exp': 133100 },
    { 'Level': 56, 'Exp': 140492 },
    { 'Level': 57, 'Exp': 148154 },
    { 'Level': 58, 'Exp': 156089 },
    { 'Level': 59, 'Exp': 164303 },
    { 'Level': 60, 'Exp': 172800 },
    { 'Level': 61, 'Exp': 181584 },
    { 'Level': 62, 'Exp': 190662 },
    { 'Level': 63, 'Exp': 200037 },
    { 'Level': 64, 'Exp': 209715 },
    { 'Level': 65, 'Exp': 219700 },
    { 'Level': 66, 'Exp': 229996 },
    { 'Level': 67, 'Exp': 240610 },
    { 'Level': 68, 'Exp': 251545 },
    { 'Level': 69, 'Exp': 262807 },
    { 'Level': 70, 'Exp': 274400 },
    { 'Level': 71, 'Exp': 286328 },
    { 'Level': 72, 'Exp': 298598 },
    { 'Level': 73, 'Exp': 311213 },
    { 'Level': 74, 'Exp': 324179 },
    { 'Level': 75, 'Exp': 337500 },
    { 'Level': 76, 'Exp': 351180 },
    { 'Level': 77, 'Exp': 365226 },
    { 'Level': 78, 'Exp': 379641 },
    { 'Level': 79, 'Exp': 394431 },
    { 'Level': 80, 'Exp': 409600 },
    { 'Level': 81, 'Exp': 425152 },
    { 'Level': 82, 'Exp': 441094 },
    { 'Level': 83, 'Exp': 457429 },
    { 'Level': 84, 'Exp': 474163 },
    { 'Level': 85, 'Exp': 491300 },
    { 'Level': 86, 'Exp': 508844 },
    { 'Level': 87, 'Exp': 526802 },
    { 'Level': 88, 'Exp': 545177 },
    { 'Level': 89, 'Exp': 563975 },
    { 'Level': 90, 'Exp': 583200 },
    { 'Level': 91, 'Exp': 602856 },
    { 'Level': 92, 'Exp': 622950 },
    { 'Level': 93, 'Exp': 643485 },
    { 'Level': 94, 'Exp': 664467 },
    { 'Level': 95, 'Exp': 685900 },
    { 'Level': 96, 'Exp': 707788 },
    { 'Level': 97, 'Exp': 730138 },
    { 'Level': 98, 'Exp': 752953 },
    { 'Level': 99, 'Exp': 776239 },
    { 'Level': 100, 'Exp': 800000 }
], mediumFastMatrix = [
    { 'Level': 1, 'Exp': 0 },
    { 'Level': 2, 'Exp': 8 },
    { 'Level': 3, 'Exp': 27 },
    { 'Level': 4, 'Exp': 64 },
    { 'Level': 5, 'Exp': 125 },
    { 'Level': 6, 'Exp': 216 },
    { 'Level': 7, 'Exp': 343 },
    { 'Level': 8, 'Exp': 512 },
    { 'Level': 9, 'Exp': 729 },
    { 'Level': 10, 'Exp': 1000 },
    { 'Level': 11, 'Exp': 1331 },
    { 'Level': 12, 'Exp': 1728 },
    { 'Level': 13, 'Exp': 2197 },
    { 'Level': 14, 'Exp': 2744 },
    { 'Level': 15, 'Exp': 3375 },
    { 'Level': 16, 'Exp': 4096 },
    { 'Level': 17, 'Exp': 4913 },
    { 'Level': 18, 'Exp': 5832 },
    { 'Level': 19, 'Exp': 6859 },
    { 'Level': 20, 'Exp': 8000 },
    { 'Level': 21, 'Exp': 9261 },
    { 'Level': 22, 'Exp': 10648 },
    { 'Level': 23, 'Exp': 12167 },
    { 'Level': 24, 'Exp': 13824 },
    { 'Level': 25, 'Exp': 15625 },
    { 'Level': 26, 'Exp': 17576 },
    { 'Level': 27, 'Exp': 19683 },
    { 'Level': 28, 'Exp': 21952 },
    { 'Level': 29, 'Exp': 24389 },
    { 'Level': 30, 'Exp': 27000 },
    { 'Level': 31, 'Exp': 29791 },
    { 'Level': 32, 'Exp': 32768 },
    { 'Level': 33, 'Exp': 35937 },
    { 'Level': 34, 'Exp': 39304 },
    { 'Level': 35, 'Exp': 42875 },
    { 'Level': 36, 'Exp': 46656 },
    { 'Level': 37, 'Exp': 50653 },
    { 'Level': 38, 'Exp': 54872 },
    { 'Level': 39, 'Exp': 59319 },
    { 'Level': 40, 'Exp': 64000 },
    { 'Level': 41, 'Exp': 68921 },
    { 'Level': 42, 'Exp': 74088 },
    { 'Level': 43, 'Exp': 79507 },
    { 'Level': 44, 'Exp': 85184 },
    { 'Level': 45, 'Exp': 91125 },
    { 'Level': 46, 'Exp': 97336 },
    { 'Level': 47, 'Exp': 103823 },
    { 'Level': 48, 'Exp': 110592 },
    { 'Level': 49, 'Exp': 117649 },
    { 'Level': 50, 'Exp': 125000 },
    { 'Level': 51, 'Exp': 132651 },
    { 'Level': 52, 'Exp': 140608 },
    { 'Level': 53, 'Exp': 148877 },
    { 'Level': 54, 'Exp': 157464 },
    { 'Level': 55, 'Exp': 166375 },
    { 'Level': 56, 'Exp': 175616 },
    { 'Level': 57, 'Exp': 185193 },
    { 'Level': 58, 'Exp': 195112 },
    { 'Level': 59, 'Exp': 205379 },
    { 'Level': 60, 'Exp': 216000 },
    { 'Level': 61, 'Exp': 226981 },
    { 'Level': 62, 'Exp': 238328 },
    { 'Level': 63, 'Exp': 250047 },
    { 'Level': 64, 'Exp': 262144 },
    { 'Level': 65, 'Exp': 274625 },
    { 'Level': 66, 'Exp': 287496 },
    { 'Level': 67, 'Exp': 300763 },
    { 'Level': 68, 'Exp': 314432 },
    { 'Level': 69, 'Exp': 328509 },
    { 'Level': 70, 'Exp': 343000 },
    { 'Level': 71, 'Exp': 357911 },
    { 'Level': 72, 'Exp': 373248 },
    { 'Level': 73, 'Exp': 389017 },
    { 'Level': 74, 'Exp': 405224 },
    { 'Level': 75, 'Exp': 421875 },
    { 'Level': 76, 'Exp': 438976 },
    { 'Level': 77, 'Exp': 456533 },
    { 'Level': 78, 'Exp': 474552 },
    { 'Level': 79, 'Exp': 493039 },
    { 'Level': 80, 'Exp': 512000 },
    { 'Level': 81, 'Exp': 531441 },
    { 'Level': 82, 'Exp': 551368 },
    { 'Level': 83, 'Exp': 571787 },
    { 'Level': 84, 'Exp': 592704 },
    { 'Level': 85, 'Exp': 614125 },
    { 'Level': 86, 'Exp': 636056 },
    { 'Level': 87, 'Exp': 658503 },
    { 'Level': 88, 'Exp': 681472 },
    { 'Level': 89, 'Exp': 704969 },
    { 'Level': 90, 'Exp': 729000 },
    { 'Level': 91, 'Exp': 753571 },
    { 'Level': 92, 'Exp': 778688 },
    { 'Level': 93, 'Exp': 804357 },
    { 'Level': 94, 'Exp': 830584 },
    { 'Level': 95, 'Exp': 857375 },
    { 'Level': 96, 'Exp': 884736 },
    { 'Level': 97, 'Exp': 912673 },
    { 'Level': 98, 'Exp': 941192 },
    { 'Level': 99, 'Exp': 970299 },
    { 'Level': 100, 'Exp': 1000000 }
], mediumSlowMatrix = [
    { "Level": 1, "Exp": 0 },
    { "Level": 2, "Exp": 9 },
    { "Level": 3, "Exp": 57 },
    { "Level": 4, "Exp": 96 },
    { "Level": 5, "Exp": 135 },
    { "Level": 6, "Exp": 179 },
    { "Level": 7, "Exp": 236 },
    { "Level": 8, "Exp": 314 },
    { "Level": 9, "Exp": 419 },
    { "Level": 10, "Exp": 560 },
    { "Level": 11, "Exp": 742 },
    { "Level": 12, "Exp": 973 },
    { "Level": 13, "Exp": 1261 },
    { "Level": 14, "Exp": 1612 },
    { "Level": 15, "Exp": 2035 },
    { "Level": 16, "Exp": 2535 },
    { "Level": 17, "Exp": 3120 },
    { "Level": 18, "Exp": 3798 },
    { "Level": 19, "Exp": 4575 },
    { "Level": 20, "Exp": 5460 },
    { "Level": 21, "Exp": 6458 },
    { "Level": 22, "Exp": 7577 },
    { "Level": 23, "Exp": 8825 },
    { "Level": 24, "Exp": 10208 },
    { "Level": 25, "Exp": 11735 },
    { "Level": 26, "Exp": 13411 },
    { "Level": 27, "Exp": 15244 },
    { "Level": 28, "Exp": 17242 },
    { "Level": 29, "Exp": 19411 },
    { "Level": 30, "Exp": 21760 },
    { "Level": 31, "Exp": 24294 },
    { "Level": 32, "Exp": 27021 },
    { "Level": 33, "Exp": 29949 },
    { "Level": 34, "Exp": 33084 },
    { "Level": 35, "Exp": 36435 },
    { "Level": 36, "Exp": 40007 },
    { "Level": 37, "Exp": 43808 },
    { "Level": 38, "Exp": 47846 },
    { "Level": 39, "Exp": 52127 },
    { "Level": 40, "Exp": 56660 },
    { "Level": 41, "Exp": 61450 },
    { "Level": 42, "Exp": 66505 },
    { "Level": 43, "Exp": 71833 },
    { "Level": 44, "Exp": 77440 },
    { "Level": 45, "Exp": 83335 },
    { "Level": 46, "Exp": 89523 },
    { "Level": 47, "Exp": 96012 },
    { "Level": 48, "Exp": 102810 },
    { "Level": 49, "Exp": 109923 },
    { "Level": 50, "Exp": 117360 },
    { "Level": 51, "Exp": 125126 },
    { "Level": 52, "Exp": 133229 },
    { "Level": 53, "Exp": 141677 },
    { "Level": 54, "Exp": 150476 },
    { "Level": 55, "Exp": 159635 },
    { "Level": 56, "Exp": 169159 },
    { "Level": 57, "Exp": 179056 },
    { "Level": 58, "Exp": 189334 },
    { "Level": 59, "Exp": 199999 },
    { "Level": 60, "Exp": 211060 },
    { "Level": 61, "Exp": 222522 },
    { "Level": 62, "Exp": 234393 },
    { "Level": 63, "Exp": 246681 },
    { "Level": 64, "Exp": 259392 },
    { "Level": 65, "Exp": 272535 },
    { "Level": 66, "Exp": 286115 },
    { "Level": 67, "Exp": 300140 },
    { "Level": 68, "Exp": 314618 },
    { "Level": 69, "Exp": 329555 },
    { "Level": 70, "Exp": 344960 },
    { "Level": 71, "Exp": 360838 },
    { "Level": 72, "Exp": 377197 },
    { "Level": 73, "Exp": 394045 },
    { "Level": 74, "Exp": 411388 },
    { "Level": 75, "Exp": 429235 },
    { "Level": 76, "Exp": 447591 },
    { "Level": 77, "Exp": 466464 },
    { "Level": 78, "Exp": 485862 },
    { "Level": 79, "Exp": 505791 },
    { "Level": 80, "Exp": 526260 },
    { "Level": 81, "Exp": 547274 },
    { "Level": 82, "Exp": 568841 },
    { "Level": 83, "Exp": 590969 },
    { "Level": 84, "Exp": 613664 },
    { "Level": 85, "Exp": 636935 },
    { "Level": 86, "Exp": 660787 },
    { "Level": 87, "Exp": 685228 },
    { "Level": 88, "Exp": 710266 },
    { "Level": 89, "Exp": 735907 },
    { "Level": 90, "Exp": 762160 },
    { "Level": 91, "Exp": 789030 },
    { "Level": 92, "Exp": 816525 },
    { "Level": 93, "Exp": 844653 },
    { "Level": 94, "Exp": 873420 },
    { "Level": 95, "Exp": 902835 },
    { "Level": 96, "Exp": 932903 },
    { "Level": 97, "Exp": 963632 },
    { "Level": 98, "Exp": 995030 },
    { "Level": 99, "Exp": 1027103 },
    { "Level": 100, "Exp": 1059860 }
], slowMatrix = [
    { 'Level': 1, 'Exp': 0 },
    { 'Level': 2, 'Exp': 10 },
    { 'Level': 3, 'Exp': 33 },
    { 'Level': 4, 'Exp': 80 },
    { 'Level': 5, 'Exp': 156 },
    { 'Level': 6, 'Exp': 270 },
    { 'Level': 7, 'Exp': 428 },
    { 'Level': 8, 'Exp': 640 },
    { 'Level': 9, 'Exp': 911 },
    { 'Level': 10, 'Exp': 1250 },
    { 'Level': 11, 'Exp': 1663 },
    { 'Level': 12, 'Exp': 2160 },
    { 'Level': 13, 'Exp': 2746 },
    { 'Level': 14, 'Exp': 3430 },
    { 'Level': 15, 'Exp': 4218 },
    { 'Level': 16, 'Exp': 5120 },
    { 'Level': 17, 'Exp': 6141 },
    { 'Level': 18, 'Exp': 7290 },
    { 'Level': 19, 'Exp': 8573 },
    { 'Level': 20, 'Exp': 10000 },
    { 'Level': 21, 'Exp': 11576 },
    { 'Level': 22, 'Exp': 13310 },
    { 'Level': 23, 'Exp': 15208 },
    { 'Level': 24, 'Exp': 17280 },
    { 'Level': 25, 'Exp': 19531 },
    { 'Level': 26, 'Exp': 21970 },
    { 'Level': 27, 'Exp': 24603 },
    { 'Level': 28, 'Exp': 27440 },
    { 'Level': 29, 'Exp': 30486 },
    { 'Level': 30, 'Exp': 33750 },
    { 'Level': 31, 'Exp': 37238 },
    { 'Level': 32, 'Exp': 40960 },
    { 'Level': 33, 'Exp': 44921 },
    { 'Level': 34, 'Exp': 49130 },
    { 'Level': 35, 'Exp': 53593 },
    { 'Level': 36, 'Exp': 58320 },
    { 'Level': 37, 'Exp': 63316 },
    { 'Level': 38, 'Exp': 68590 },
    { 'Level': 39, 'Exp': 74148 },
    { 'Level': 40, 'Exp': 80000 },
    { 'Level': 41, 'Exp': 86151 },
    { 'Level': 42, 'Exp': 92610 },
    { 'Level': 43, 'Exp': 99383 },
    { 'Level': 44, 'Exp': 106480 },
    { 'Level': 45, 'Exp': 113906 },
    { 'Level': 46, 'Exp': 121670 },
    { 'Level': 47, 'Exp': 129778 },
    { 'Level': 48, 'Exp': 138240 },
    { 'Level': 49, 'Exp': 147061 },
    { 'Level': 50, 'Exp': 156250 },
    { 'Level': 51, 'Exp': 165813 },
    { 'Level': 52, 'Exp': 175760 },
    { 'Level': 53, 'Exp': 186096 },
    { 'Level': 54, 'Exp': 196830 },
    { 'Level': 55, 'Exp': 207968 },
    { 'Level': 56, 'Exp': 219520 },
    { 'Level': 57, 'Exp': 231491 },
    { 'Level': 58, 'Exp': 243890 },
    { 'Level': 59, 'Exp': 256723 },
    { 'Level': 60, 'Exp': 270000 },
    { 'Level': 61, 'Exp': 283726 },
    { 'Level': 62, 'Exp': 297910 },
    { 'Level': 63, 'Exp': 312558 },
    { 'Level': 64, 'Exp': 327680 },
    { 'Level': 65, 'Exp': 343281 },
    { 'Level': 66, 'Exp': 359370 },
    { 'Level': 67, 'Exp': 375953 },
    { 'Level': 68, 'Exp': 393040 },
    { 'Level': 69, 'Exp': 410636 },
    { 'Level': 70, 'Exp': 428750 },
    { 'Level': 71, 'Exp': 447388 },
    { 'Level': 72, 'Exp': 466560 },
    { 'Level': 73, 'Exp': 486271 },
    { 'Level': 74, 'Exp': 506530 },
    { 'Level': 75, 'Exp': 527343 },
    { 'Level': 76, 'Exp': 548720 },
    { 'Level': 77, 'Exp': 570666 },
    { 'Level': 78, 'Exp': 593190 },
    { 'Level': 79, 'Exp': 616298 },
    { 'Level': 80, 'Exp': 640000 },
    { 'Level': 81, 'Exp': 664301 },
    { 'Level': 82, 'Exp': 689210 },
    { 'Level': 83, 'Exp': 714733 },
    { 'Level': 84, 'Exp': 740880 },
    { 'Level': 85, 'Exp': 767656 },
    { 'Level': 86, 'Exp': 795070 },
    { 'Level': 87, 'Exp': 823128 },
    { 'Level': 88, 'Exp': 851840 },
    { 'Level': 89, 'Exp': 881211 },
    { 'Level': 90, 'Exp': 911250 },
    { 'Level': 91, 'Exp': 941963 },
    { 'Level': 92, 'Exp': 973360 },
    { 'Level': 93, 'Exp': 1005446 },
    { 'Level': 94, 'Exp': 1038230 },
    { 'Level': 95, 'Exp': 1071718 },
    { 'Level': 96, 'Exp': 1105920 },
    { 'Level': 97, 'Exp': 1140841 },
    { 'Level': 98, 'Exp': 1176490 },
    { 'Level': 99, 'Exp': 1212873 },
    { 'Level': 100, 'Exp': 1250000 }
], fluctuatingMatrix = [
    { 'Level': 1, 'Exp': 0 },
    { 'Level': 2, 'Exp': 4 },
    { 'Level': 3, 'Exp': 13 },
    { 'Level': 4, 'Exp': 32 },
    { 'Level': 5, 'Exp': 65 },
    { 'Level': 6, 'Exp': 112 },
    { 'Level': 7, 'Exp': 178 },
    { 'Level': 8, 'Exp': 276 },
    { 'Level': 9, 'Exp': 393 },
    { 'Level': 10, 'Exp': 540 },
    { 'Level': 11, 'Exp': 745 },
    { 'Level': 12, 'Exp': 967 },
    { 'Level': 13, 'Exp': 1230 },
    { 'Level': 14, 'Exp': 1591 },
    { 'Level': 15, 'Exp': 1957 },
    { 'Level': 16, 'Exp': 2457 },
    { 'Level': 17, 'Exp': 3046 },
    { 'Level': 18, 'Exp': 3732 },
    { 'Level': 19, 'Exp': 4526 },
    { 'Level': 20, 'Exp': 5440 },
    { 'Level': 21, 'Exp': 6482 },
    { 'Level': 22, 'Exp': 7666 },
    { 'Level': 23, 'Exp': 9003 },
    { 'Level': 24, 'Exp': 10506 },
    { 'Level': 25, 'Exp': 12187 },
    { 'Level': 26, 'Exp': 14060 },
    { 'Level': 27, 'Exp': 16140 },
    { 'Level': 28, 'Exp': 18439 },
    { 'Level': 29, 'Exp': 20974 },
    { 'Level': 30, 'Exp': 23760 },
    { 'Level': 31, 'Exp': 26811 },
    { 'Level': 32, 'Exp': 30146 },
    { 'Level': 33, 'Exp': 33780 },
    { 'Level': 34, 'Exp': 37731 },
    { 'Level': 35, 'Exp': 42017 },
    { 'Level': 36, 'Exp': 46656 },
    { 'Level': 37, 'Exp': 50653 },
    { 'Level': 38, 'Exp': 55969 },
    { 'Level': 39, 'Exp': 60505 },
    { 'Level': 40, 'Exp': 66560 },
    { 'Level': 41, 'Exp': 71677 },
    { 'Level': 42, 'Exp': 78533 },
    { 'Level': 43, 'Exp': 84277 },
    { 'Level': 44, 'Exp': 91998 },
    { 'Level': 45, 'Exp': 98415 },
    { 'Level': 46, 'Exp': 107069 },
    { 'Level': 47, 'Exp': 114205 },
    { 'Level': 48, 'Exp': 123863 },
    { 'Level': 49, 'Exp': 131766 },
    { 'Level': 50, 'Exp': 142500 },
    { 'Level': 51, 'Exp': 151222 },
    { 'Level': 52, 'Exp': 163105 },
    { 'Level': 53, 'Exp': 172697 },
    { 'Level': 54, 'Exp': 185807 },
    { 'Level': 55, 'Exp': 196322 },
    { 'Level': 56, 'Exp': 210739 },
    { 'Level': 57, 'Exp': 222231 },
    { 'Level': 58, 'Exp': 238036 },
    { 'Level': 59, 'Exp': 250562 },
    { 'Level': 60, 'Exp': 267840 },
    { 'Level': 61, 'Exp': 281456 },
    { 'Level': 62, 'Exp': 300293 },
    { 'Level': 63, 'Exp': 315059 },
    { 'Level': 64, 'Exp': 335544 },
    { 'Level': 65, 'Exp': 351520 },
    { 'Level': 66, 'Exp': 373744 },
    { 'Level': 67, 'Exp': 390991 },
    { 'Level': 68, 'Exp': 415050 },
    { 'Level': 69, 'Exp': 433631 },
    { 'Level': 70, 'Exp': 459620 },
    { 'Level': 71, 'Exp': 479600 },
    { 'Level': 72, 'Exp': 507617 },
    { 'Level': 73, 'Exp': 529063 },
    { 'Level': 74, 'Exp': 559209 },
    { 'Level': 75, 'Exp': 582187 },
    { 'Level': 76, 'Exp': 614566 },
    { 'Level': 77, 'Exp': 639146 },
    { 'Level': 78, 'Exp': 673863 },
    { 'Level': 79, 'Exp': 700115 },
    { 'Level': 80, 'Exp': 737280 },
    { 'Level': 81, 'Exp': 765275 },
    { 'Level': 82, 'Exp': 804997 },
    { 'Level': 83, 'Exp': 834809 },
    { 'Level': 84, 'Exp': 877201 },
    { 'Level': 85, 'Exp': 908905 },
    { 'Level': 86, 'Exp': 954084 },
    { 'Level': 87, 'Exp': 987754 },
    { 'Level': 88, 'Exp': 1035837 },
    { 'Level': 89, 'Exp': 1071552 },
    { 'Level': 90, 'Exp': 1122660 },
    { 'Level': 91, 'Exp': 1160499 },
    { 'Level': 92, 'Exp': 1214753 },
    { 'Level': 93, 'Exp': 1254796 },
    { 'Level': 94, 'Exp': 1312322 },
    { 'Level': 95, 'Exp': 1354652 },
    { 'Level': 96, 'Exp': 1415577 },
    { 'Level': 97, 'Exp': 1460276 },
    { 'Level': 98, 'Exp': 1524731 },
    { 'Level': 99, 'Exp': 1571884 },
    { 'Level': 100, 'Exp': 1640000 }
];

var calculateMaxExpPoints = function (expGroup) {
    expGroupId = expGroup;
    if (expGroupId == 1) {
        maximumExpPoints = 600000;
    } else if (expGroupId == 2) {
        maximumExpPoints = 800000;
    } else if (expGroupId == 3) {
        maximumExpPoints = 1000000;
    } else if (expGroupId == 4) {
        maximumExpPoints = 1059860;
    } else if (expGroupId == 5) {
        maximumExpPoints = 1250000;
    } else if (expGroupId == 6) {
        maximumExpPoints = 1640000;
    }

    $('.expObtained').attr("max", maximumExpPoints);

    if ($('.expObtained').val() > maximumExpPoints) {
        $('.expObtained').val(maximumExpPoints);
    }
}, calculateLevelFromExp = function (experience) {
    var expMatrix = [];
    if (expGroupId == 1) {
        expMatrix = erraticMatrix;
    } else if (expGroupId == 2) {
        expMatrix = fastMatrix;
    } else if (expGroupId == 3) {
        expMatrix = mediumFastMatrix;
    } else if (expGroupId == 4) {
        expMatrix = mediumSlowMatrix;
    } else if (expGroupId == 5) {
        expMatrix = slowMatrix;
    } else if (expGroupId == 6) {
        expMatrix = fluctuatingMatrix;
    }
    
    for (var i = 1; i <= 100; i++) {
        if (expMatrix[i].Exp >= experience) {
            minimumLevel = expMatrix[i].Level;
            if (expMatrix[i].Exp == experience && expMatrix[i].Level != 100) {
                minimumLevel = expMatrix[i].Level + 1;
            }
            break;
        }
    };

    $('.requestedLevel').attr("min", minimumLevel);

    if ($('.requestedLevel').val() < minimumLevel) {
        $('.requestedLevel').val(minimumLevel);
    }
}, calculateExpNeeded = function (expObtained, levelNeeded, expGroup) {
    if (levelNeeded >= minimumLevel && expGroup != NaN) {
        var expMatrix = [], expNeeded = 0, xlCandiesNeeded = 0, lCandiesNeeded = 0, mCandiesNeeded = 0, sCandiesNeeded = 0, xsCandiesNeeded = 0;
        if (expGroupId == 1) {
            expMatrix = erraticMatrix;
        } else if (expGroupId == 2) {
            expMatrix = fastMatrix;
        } else if (expGroupId == 3) {
            expMatrix = mediumFastMatrix;
        } else if (expGroupId == 4) {
            expMatrix = mediumSlowMatrix;
        } else if (expGroupId == 5) {
            expMatrix = slowMatrix;
        } else if (expGroupId == 6) {
            expMatrix = fluctuatingMatrix;
        }

        expNeeded = expMatrix[levelNeeded - 1].Exp - expObtained;
        if (Math.trunc(expNeeded / 30000) > 0) {
            xlCandiesNeeded = Math.trunc(expNeeded / 30000);
            expNeeded -= (xlCandiesNeeded * 30000);
        }
        if (Math.trunc(expNeeded / 10000) > 0) {
            lCandiesNeeded = Math.trunc(expNeeded / 10000);
            expNeeded -= (lCandiesNeeded * 10000);
        }
        if (Math.trunc(expNeeded / 3000) > 0) {
            mCandiesNeeded = Math.trunc(expNeeded / 3000);
            expNeeded -= (mCandiesNeeded * 3000);
        }
        if (Math.trunc(expNeeded / 800) > 0) {
            sCandiesNeeded = Math.trunc(expNeeded / 800);
            expNeeded -= (sCandiesNeeded * 800);
        }
        if (Math.trunc(expNeeded / 100) > 0) {
            xsCandiesNeeded = Math.trunc(expNeeded / 100);
        }

        if (expNeeded > 0) {
            xsCandiesNeeded++;
            if (xsCandiesNeeded == 8) {
                sCandiesNeeded++;
                xsCandiesNeeded = 0;
            }
        }
        
        $('.xsCandy .candyAmount').html(xsCandiesNeeded);
        $('.sCandy .candyAmount').html(sCandiesNeeded);
        $('.mCandy .candyAmount').html(mCandiesNeeded);
        $('.lCandy .candyAmount').html(lCandiesNeeded);
        $('.xlCandy .candyAmount').html(xlCandiesNeeded);
    }
};

$(function () {
    calculateMaxExpPoints($('.pokemonSelectList').val());
    if ($('.expObtained').val() > 0) {
        calculateLevelFromExp($('.expObtained').val());
    }
});

$(".pokemonSelectList").on('change', function () {
    calculateMaxExpPoints($('.pokemonSelectList').val());
    if ($('.expObtained').val() > 0) {
        calculateLevelFromExp($('.expObtained').val());
    }
    calculateExpNeeded(parseInt($('.expObtained').val()), parseInt($('.requestedLevel').val()), parseInt($('.pokemonSelectList').val()));
});

$(".expObtained").on('change', function () {
    if ($('.expObtained').val() > maximumExpPoints) {
        $('.expObtained').val(maximumExpPoints);
    }
    if ($(".pokemonSelectList").val() != "") {
        calculateLevelFromExp($('.expObtained').val());
    }
    calculateExpNeeded(parseInt($('.expObtained').val()), parseInt($('.requestedLevel').val()), parseInt($('.pokemonSelectList').val()));
});

$(".requestedLevel").on('change', function () {
    if ($('.requestedLevel').val() < minimumLevel) {
        $('.requestedLevel').val(minimumLevel);
    }
    calculateExpNeeded(parseInt($('.expObtained').val()), parseInt($('.requestedLevel').val()), parseInt($('.pokemonSelectList').val()));
});