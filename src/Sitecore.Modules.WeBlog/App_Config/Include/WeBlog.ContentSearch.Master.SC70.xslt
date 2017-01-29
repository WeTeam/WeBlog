<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/configuration/sitecore/contentSearch/configuration/indexes/index[@id='WeBlog-master']/param[@desc='propertyStore']/@ref">
    <xsl:attribute name="ref">contentSearch/databasePropertyStore</xsl:attribute>
  </xsl:template>

  <xsl:template match="/configuration/sitecore/contentSearch/configuration/indexes/index[@id='WeBlog-master']/strategies/strategy[@ref='contentSearch/indexConfigurations/indexUpdateStrategies/syncMaster']/@ref">
    <xsl:attribute name="ref">contentSearch/indexUpdateStrategies/syncMaster</xsl:attribute>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
