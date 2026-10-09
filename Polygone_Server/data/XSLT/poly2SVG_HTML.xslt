<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:pol="http://www.l3miage.fr/geometry">
    <xsl:output method="html"/>
    
    <xsl:template match="/">
        <html>
            <head>
                <title>Polygone vers SVG HTML</title>
                <style>
                    body {background-color: powderblue;}
                    h1   {color: blue;}
                    p    {color: red;}
                </style>
            </head>
            <body>
                <div>
                    <h1>Transformation Polygone vers SVG</h1>
                    <h2>Nb sommets du polygone = <xsl:value-of select="count(//pol:sommet)"/></h2>
                    <svg height="210" width="500" xmlns="http://www.w3.org/2000/svg">
                        <xsl:element name="polygon">
                            <xsl:attribute name="points"><xsl:apply-templates select="//pol:polygone"/></xsl:attribute>
                            <xsl:attribute name="style">fill:lime;stroke:purple;stroke-width:5;fill-rule:evenodd;</xsl:attribute>
                        </xsl:element>
                    </svg>
                </div>
            </body>
        </html>
    </xsl:template>
    
    <xsl:template match="pol:polygone">
        <xsl:param name="nbSommets" select="count(pol:sommet)"/>
        <xsl:apply-templates select="pol:sommet">
            <xsl:with-param name="nb_Sommets" select="$nbSommets"/>
        </xsl:apply-templates>
    </xsl:template>
    
    <xsl:template match="pol:sommet">
        <xsl:param name="nb_sommet"/>
        <xsl:value-of select="pol:x"/>,<xsl:value-of select="pol:y"/>
        <xsl:choose>
            <xsl:when test="$nb_sommet > position()"> <xsl:text> </xsl:text></xsl:when>
            <xsl:otherwise></xsl:otherwise>
        </xsl:choose>
       
    </xsl:template>
    
</xsl:stylesheet>