# UnityBatAttacksSimpleGame

Задача

На сцене присутствует 3 летучих мыши. Они появляются в рандомной точке, на высоте от 15 до 35
метров. На расстоянии 20-100 метров от камеры.
Для мыши формируется целевая точка  -
место в которое в дальнейшем полетит мышь. Это новая точка с рандомными координатами с
теми же ограничениями.
Мышь начинает свой полет через случаайный интервал времени - каждая мышь через свой, от 1 до
10 секунд.
Мышь летит по некоторой кривой, подразумевающей естественность полета. Далее ударяет
условного игрока и улетает на место целевой сферы.
Когда мышь прилетает к сфере, сфера исчезает и генерируется новая в новом месте. И для мыши
все начинается сначала.
После атаки мыши, также происходит покраяснение экрана, плавно исчезающее в течение
полутора секунд.
Кроме того, должна быть возможность поворота камеры с клавиатуры. Доплнительно должна быть возможность двигаться AWSD. В том случае если условный
персонаж увернулся, атака не происходить - мышь пролетает дальше.

Реализация 

Для управления поведением летучей мыши был написан класс BatController, имеющий несколько состояний, каждое из которых выполняет свою 
функцию. Starting - в этом состоянии генерируются целевая точка и время ожидания. Wait - в этом состоянии мышь ждёт окончания времени 
ожидания. FlyingToPlayer - в этом состоянии мышь каждые две секунды запоминает последнюю позицию игрока и летит в неё. Если она 
во время полета задевает игрока, то переходит в состояние Attacking, иначе в состояние Returning. Attacking - состояние, в котором мышь 
ожидает конца анимации атаки. Returning - состояние, в котором мышь летит в целевую точку. После состояния Returning мышь переходит 
в состояние Starting.

Также для управлением игроком был написан класс CamController, который включает управление поворотом и положением камеры. Ещё в нем 
содержится логика покраснения экрана в момент, когда мышь задевает игрока.