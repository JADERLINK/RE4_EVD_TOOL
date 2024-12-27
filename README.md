# RE4-EVD-TOOL
Extract and repack RE4 EVD files (RE4 2007/PS2/UHD/PS4/NS/GC/WII/XBOX360)

**Translate from Portuguese Brazil**

Programa destinado a extrair e reempacotar arquivos .EVD
<br> Ao extrair será gerado um arquivo de extenção .idxevd, ele será usado para o repack.

## Extract

Exemplo:
<br>RE4_EVD_TOOL_**.exe "r100s00.evd"

! Vai gerar um arquivo de nome "r100s00.idxevd";
<br>! Vai criar uma pasta de nome "r100s00";
<br>! Na pasta vão conter os arquivos que estavam dentro do EVD;
<br>! E na pasta vai ter também um arquivo de nome "r100s00.evdhdr" ou "r100s00.evdhdrbig"

## Repack

Exemplo:
<br>RE4_EVD_TOOL_**.exe "r100s00.idxevd"

! No arquivo .idxevd vai conter os nomes dos arquivos que vão ser colocados no EVD;
<br>! Os arquivos têm que estar em uma pasta do mesmo nome do idxevd. Ex: "r100s00";
<br>! No arquivo .idxevd as linhas iniciadas com um dos caracteres **# / \\ :** são consideradas comentários e não arquivos;
<br>! O nome do arquivo gerado é o mesmo nome do idxevd, mas com a extenção .evd;
<br>! O Arquivo "r100s00.evdhdr" ou "r100s00.evdhdrbig" é necessário para o repack;

## BIG_ENDIAN vs LITTLE_ENDIAN

! Para as versões "GC/WII/XBOX360" use a tool de nome BIG_ENDIAN;
<br>! Para as versões "2007/PS2/UHD/PS4/NS" use a tool de nome LITTLE_ENDIAN;

## BASIC vs DIFF

Muda a forma de como os arquivos são extraídos do EVD, o "BASIC" vai usar o tamanho do arquivo informado no evd, e o "DIFF" vai usar a diferença entre os offsets de um arquivo para outro.
Fiz duas maneiras de extrair os arquivos, pois o arquivo evd é uma bagunça, e nem sempre o tamanho informado para o arquivo, é o tamanho real do mesmo;
<br>Você vai ter que testar qual tool extrai melhor o arquivo evd que você pretende extrair;

## EVDHDR and EVDHDRBIG

Esse é o arquivo que contém uma parte do header do arquivo EVD, no qual não pode ser editado com essa tool;
<br>! O EVDHDR é um arquivo LITTLE ENDIAN;
<br>! E o EVDHDRBIG é um arquivo BIG ENDIAN;

**At.te: JADERLINK**
<br>2024-12-27